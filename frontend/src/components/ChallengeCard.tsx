import { useState, type FormEvent } from 'react'
import { Link } from 'react-router-dom'
import { getErrorMessage } from '../api/errors'
import { abandonChallenge, startChallenge } from '../api/goals'
import type { Challenge } from '../api/types'
import { addMinutes, fromDateAndTime, toDateInputValue, toLocalIso } from '../utils/dates'
import { repeatLabels } from '../utils/rewards'
import { Alert, Button, TextField } from './Form'

interface ChallengeCardProps {
  challenge: Challenge
  /** Kaldes efter start eller opgiv, så siden kan hente udfordringerne igen. */
  onChanged: () => void
}

function tomorrow(): string {
  const date = new Date()
  date.setDate(date.getDate() + 1)
  return toDateInputValue(date)
}

export default function ChallengeCard({ challenge, onChanged }: ChallengeCardProps) {
  const [starting, setStarting] = useState(false)
  const [date, setDate] = useState(tomorrow)
  const [startTime, setStartTime] = useState('07:00')
  const [endTime, setEndTime] = useState('07:30')
  const [confirmAbandon, setConfirmAbandon] = useState(false)
  const [busy, setBusy] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const { status, progress, requiredCount } = challenge
  const percent = Math.round((progress / requiredCount) * 100)

  async function handleStart(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setBusy(true)

    try {
      const start = fromDateAndTime(date, startTime)
      const end = endTime ? fromDateAndTime(date, endTime) : null
      await startChallenge(challenge.id, toLocalIso(start), end ? toLocalIso(end) : null)
      setStarting(false)
      onChanged()
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setBusy(false)
    }
  }

  async function handleAbandon() {
    setError(null)
    setBusy(true)

    try {
      await abandonChallenge(challenge.id)
      setConfirmAbandon(false)
      onChanged()
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setBusy(false)
    }
  }

  return (
    <div className={`card h-100${status === 'Completed' ? ' border-success' : status === 'Active' ? ' border-primary' : ''}`}>
      <div className="card-body">
        <div className="d-flex justify-content-between align-items-start gap-2 mb-1">
          <h3 className="h6 card-title mb-0">{challenge.name}</h3>
          {status === 'Completed' && <span className="badge text-bg-success">Gennemført</span>}
          {status === 'Active' && <span className="badge text-bg-primary">I gang</span>}
        </div>
        <p className="card-text small text-body-secondary mb-2">{challenge.description}</p>
        <p className="small mb-2">
          "{challenge.taskTitle}" {repeatLabels[challenge.repeat]} – {requiredCount} gange
        </p>
        <div className="d-flex gap-2 mb-2">
          <span className="badge text-bg-primary">+{challenge.rewardXp} XP</span>
          <span className="badge text-bg-warning">+{challenge.rewardCoins} points</span>
        </div>

        {error && <Alert>{error}</Alert>}

        {status !== 'NotStarted' && (
          <>
            <div className="d-flex justify-content-between small mb-1">
              <span>
                {progress} af {requiredCount} gennemført
              </span>
            </div>
            <div className="progress mb-2" role="progressbar" aria-valuemin={0} aria-valuemax={requiredCount} aria-valuenow={progress}>
              <div className="progress-bar bg-success" style={{ width: `${percent}%` }} />
            </div>
          </>
        )}

        {starting && (
          <form onSubmit={handleStart} className="mt-3">
            <p className="small text-body-secondary mb-2">
              Vælg hvornår den første opgave skal ligge. Resten lægges {repeatLabels[challenge.repeat]} efter.
            </p>
            {/* Kortet er smalt, så datoen får sin egen linje og tidspunkterne deler den næste */}
            <div className="row g-2">
              <div className="col-12">
                <TextField label="Første dag" type="date" value={date} onChange={(e) => setDate(e.target.value)} required />
              </div>
              <div className="col-6">
                <TextField
                  label="Fra"
                  type="time"
                  value={startTime}
                  onChange={(e) => {
                    setStartTime(e.target.value)
                    // Hold samme varighed, hvis sluttidspunktet ellers ville ligge før start
                    if (e.target.value && endTime && endTime <= e.target.value) {
                      const end = addMinutes(fromDateAndTime(date, e.target.value), 30)
                      setEndTime(`${String(end.getHours()).padStart(2, '0')}:${String(end.getMinutes()).padStart(2, '0')}`)
                    }
                  }}
                  required
                />
              </div>
              <div className="col-6">
                <TextField label="Til" type="time" value={endTime} onChange={(e) => setEndTime(e.target.value)} />
              </div>
            </div>
            <div className="d-flex gap-2">
              <Button type="submit" className="btn-sm" loading={busy}>
                Sæt i kalenderen
              </Button>
              <Button type="button" variant="link" className="btn-sm" onClick={() => setStarting(false)}>
                Annuller
              </Button>
            </div>
          </form>
        )}
      </div>

      <div className="card-footer bg-transparent">
        {status === 'NotStarted' && !starting && (
          <Button className="btn-sm" onClick={() => setStarting(true)}>
            Start udfordring
          </Button>
        )}

        {status === 'Active' && !confirmAbandon && (
          <div className="d-flex align-items-center gap-2">
            <Link to="/week-plan" className="btn btn-outline-primary btn-sm">
              Se i ugeplanen
            </Link>
            <Button variant="link" className="btn-sm text-danger ms-auto" onClick={() => setConfirmAbandon(true)}>
              Opgiv
            </Button>
          </div>
        )}

        {status === 'Active' && confirmAbandon && (
          <div className="d-flex flex-wrap align-items-center gap-2">
            <span className="small me-auto">Opgiv? Ikke-gennemførte opgaver fjernes fra kalenderen.</span>
            <Button variant="danger" className="btn-sm" onClick={handleAbandon} loading={busy}>
              Ja, opgiv
            </Button>
            <Button variant="link" className="btn-sm" onClick={() => setConfirmAbandon(false)} disabled={busy}>
              Nej
            </Button>
          </div>
        )}

        {status === 'Completed' && challenge.completedAt && (
          <span className="small text-success fw-semibold">
            Gennemført {new Date(challenge.completedAt).toLocaleDateString('da-DK')}
          </span>
        )}
      </div>
    </div>
  )
}
