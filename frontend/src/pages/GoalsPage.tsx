import { useCallback, useEffect, useState } from 'react'
import { getErrorMessage } from '../api/errors'
import { getAchievements, getChallenges } from '../api/goals'
import type { Achievement, Challenge } from '../api/types'
import ChallengeCard from '../components/ChallengeCard'
import { Alert } from '../components/Form'
import GameIcon from '../components/GameIcon'
import { achievementIcons } from '../utils/rewards'

/** Achievements (mål der låses op automatisk) og udfordringer (forløb man sætter i kalenderen). */
export default function GoalsPage() {
  const [achievements, setAchievements] = useState<Achievement[] | null>(null)
  const [challenges, setChallenges] = useState<Challenge[] | null>(null)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(() => {
    Promise.all([getAchievements(), getChallenges()])
      .then(([a, c]) => {
        setAchievements(a)
        setChallenges(c)
      })
      .catch((err) => setError(getErrorMessage(err)))
  }, [])

  useEffect(load, [load])

  if (!achievements || !challenges) {
    return error ? <Alert>{error}</Alert> : <p className="text-body-secondary">Henter mål…</p>
  }

  const unlockedCount = achievements.filter((a) => a.unlockedAt).length

  return (
    <>
      {error && <Alert>{error}</Alert>}

      <section className="mb-5">
        <div className="d-flex flex-wrap justify-content-between align-items-baseline gap-1 mb-3">
          <h2 className="h5 mb-0">Udfordringer</h2>
          <span className="small text-body-secondary">Start en udfordring for at få den i din kalender</span>
        </div>
        <div className="row g-3">
          {challenges.map((challenge) => (
            <div key={challenge.id} className="col-12 col-md-6 col-xl-3">
              <ChallengeCard challenge={challenge} onChanged={load} />
            </div>
          ))}
        </div>
      </section>

      <section>
        <div className="d-flex flex-wrap justify-content-between align-items-baseline gap-1 mb-3">
          <h2 className="h5 mb-0">Achievements</h2>
          <span className="small text-body-secondary">
            {unlockedCount} af {achievements.length} låst op
          </span>
        </div>
        <div className="row g-3">
          {achievements.map((achievement) => {
            const unlocked = achievement.unlockedAt !== null
            const percent = Math.round((achievement.currentCount / achievement.requiredCount) * 100)

            return (
              <div key={achievement.id} className="col-6 col-md-4 col-xl-3">
                <div className={`card h-100 text-center${unlocked ? ' border-success' : ''}`}>
                  <div className="card-body">
                    <GameIcon
                      name={achievementIcons[achievement.type]}
                      className={`fs-1 mb-1${unlocked ? ' text-success' : ' opacity-25'}`}
                    />
                    <h3 className="h6 mb-1">{achievement.name}</h3>
                    <p className="small text-body-secondary mb-2">{achievement.description}</p>

                    {unlocked ? (
                      <span className="small text-success fw-semibold">
                        <GameIcon name="trophy" /> Låst op {new Date(achievement.unlockedAt!).toLocaleDateString('da-DK')}
                      </span>
                    ) : (
                      <>
                        <div className="progress" style={{ height: 6 }}>
                          <div className="progress-bar" style={{ width: `${percent}%` }} />
                        </div>
                        <span className="small text-body-secondary">
                          {achievement.currentCount} / {achievement.requiredCount}
                        </span>
                      </>
                    )}
                  </div>
                </div>
              </div>
            )
          })}
        </div>
      </section>
    </>
  )
}
