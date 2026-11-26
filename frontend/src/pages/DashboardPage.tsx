import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { api } from '../api/client'
import { getErrorMessage } from '../api/errors'
import { completeTask, getTasks } from '../api/tasks'
import type { TaskItem } from '../api/types'
import { useAuth } from '../auth/useAuth'
import { Alert, Button } from '../components/Form'
import { fromLocalIso, toLocalIso, toTimeInputValue } from '../utils/dates'
import { rewardMessage } from '../utils/rewards'

type ApiStatus = 'tjekker…' | 'online' | 'offline'

/** I dag fra kl. 00:00 til i morgen kl. 00:00 (lokal tid). */
function todayRange(): { from: string; to: string } {
  const start = new Date()
  start.setHours(0, 0, 0, 0)
  const end = new Date(start)
  end.setDate(end.getDate() + 1)
  return { from: toLocalIso(start), to: toLocalIso(end) }
}

function timeText(task: TaskItem): string {
  const start = toTimeInputValue(fromLocalIso(task.scheduledStart))
  return task.scheduledEnd ? `${start}–${toTimeInputValue(fromLocalIso(task.scheduledEnd))}` : start
}

export default function DashboardPage() {
  const { profile, refreshProfile } = useAuth()
  const [tasks, setTasks] = useState<TaskItem[] | null>(null)
  const [completingId, setCompletingId] = useState<number | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [reward, setReward] = useState<string | null>(null)
  const [status, setStatus] = useState<ApiStatus>('tjekker…')
  // Beregnes én gang, når siden åbnes – ligesom opgaverne
  const [today] = useState(() => {
    const text = new Date().toLocaleDateString('da-DK', { weekday: 'long', day: 'numeric', month: 'long' })
    return text.charAt(0).toUpperCase() + text.slice(1) // "Torsdag 26. november"
  })

  useEffect(() => {
    const { from, to } = todayRange()
    getTasks(from, to)
      .then(setTasks)
      .catch((err) => setError(getErrorMessage(err)))

    api
      .get('/health')
      .then(() => setStatus('online'))
      .catch(() => setStatus('offline'))
  }, [])

  async function handleComplete(task: TaskItem) {
    setError(null)
    setCompletingId(task.id)

    try {
      const result = await completeTask(task.id)
      setTasks((current) => current?.map((t) => (t.id === task.id ? result.task : t)) ?? null)
      setReward(rewardMessage(result))
      await refreshProfile()
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setCompletingId(null)
    }
  }

  const completedCount = tasks?.filter((t) => t.status === 'Completed').length ?? 0
  const xpPercent = profile ? Math.round((profile.xpIntoLevel / profile.xpForNextLevel) * 100) : 0

  return (
    <div className="row g-3">
      <div className="col-lg-8">
        <div className="card h-100">
          <div className="card-header d-flex justify-content-between align-items-center">
            <span className="fw-semibold">Dagens opgaver</span>
            <span className="small text-body-secondary">{today}</span>
          </div>

          {(error || reward) && (
            <div className="card-body pb-0">
              {error && <Alert>{error}</Alert>}
              {reward && (
                <div className="alert alert-success alert-dismissible py-2" role="status">
                  {reward}
                  <button type="button" className="btn-close py-2" aria-label="Luk" onClick={() => setReward(null)} />
                </div>
              )}
            </div>
          )}

          {!tasks ? (
            <div className="card-body text-body-secondary">{error ? null : 'Henter opgaver…'}</div>
          ) : tasks.length === 0 ? (
            <div className="card-body">
              <p className="text-body-secondary mb-2">Du har ingen opgaver i dag.</p>
              <Link to="/ugeplan" className="btn btn-primary btn-sm">
                Planlæg i ugeplanen
              </Link>
            </div>
          ) : (
            <>
              <div className="card-body pb-2">
                <div className="d-flex justify-content-between small mb-1">
                  <span>
                    {completedCount} af {tasks.length} gennemført
                  </span>
                </div>
                <div
                  className="progress"
                  role="progressbar"
                  aria-label="Dagens fremskridt"
                  aria-valuemin={0}
                  aria-valuemax={tasks.length}
                  aria-valuenow={completedCount}
                >
                  <div
                    className="progress-bar bg-success"
                    style={{ width: `${Math.round((completedCount / tasks.length) * 100)}%` }}
                  />
                </div>
              </div>

              <ul className="list-group list-group-flush">
                {tasks.map((task) => {
                  const completed = task.status === 'Completed'

                  return (
                    <li key={task.id} className="list-group-item d-flex align-items-center gap-3">
                      <span className="small text-body-secondary text-nowrap" style={{ minWidth: 90 }}>
                        {timeText(task)}
                      </span>
                      <div className="me-auto">
                        <div className={completed ? 'text-decoration-line-through text-body-secondary' : 'fw-semibold'}>
                          {task.seriesId && <span aria-label="Gentagende">↻ </span>}
                          {task.title}
                        </div>
                        {task.description && <div className="small text-body-secondary">{task.description}</div>}
                      </div>
                      {completed ? (
                        <span className="badge text-bg-success">Gennemført</span>
                      ) : (
                        <Button
                          variant="success"
                          className="btn-sm text-nowrap"
                          onClick={() => handleComplete(task)}
                          loading={completingId === task.id}
                          disabled={completingId !== null}
                        >
                          Gennemfør
                        </Button>
                      )}
                    </li>
                  )
                })}
              </ul>
            </>
          )}
        </div>
      </div>

      <div className="col-lg-4 d-flex flex-column gap-3">
        <div className="card">
          <div className="card-body">
            <h2 className="h6 card-title">Fremskridt</h2>
            {profile ? (
              <>
                <p className="mb-1">
                  <span className="badge text-bg-primary me-2">Level {profile.level}</span>
                  <span className="badge text-bg-warning">{profile.coinBalance} points</span>
                </p>
                <div className="progress my-2" style={{ height: 8 }}>
                  <div className="progress-bar bg-success" style={{ width: `${xpPercent}%` }} />
                </div>
                <p className="small text-body-secondary mb-0">
                  {profile.xpForNextLevel - profile.xpIntoLevel} XP til level {profile.level + 1}
                </p>
              </>
            ) : (
              <p className="card-text text-body-secondary">Henter…</p>
            )}
          </div>
        </div>

        <div className="card">
          <div className="card-body">
            <h2 className="h6 card-title">System</h2>
            <p className="card-text text-body-secondary small mb-0">API status: {status}</p>
          </div>
        </div>
      </div>
    </div>
  )
}
