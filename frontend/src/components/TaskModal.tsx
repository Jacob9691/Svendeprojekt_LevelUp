import { useEffect, useId, useState, type FormEvent } from 'react'
import { getErrorMessage, getFieldErrors } from '../api/errors'
import { completeTask, createTask, deleteTask, updateTask } from '../api/tasks'
import type { CompleteTaskResponse, RecurrenceFrequency, TaskItem } from '../api/types'
import { fromDateAndTime, fromLocalIso, toDateInputValue, toLocalIso, toTimeInputValue } from '../utils/dates'
import { Alert, Button, SelectField, TextAreaField, TextField } from './Form'

const repeatOptions: { value: RecurrenceFrequency | ''; label: string }[] = [
  { value: '', label: 'Gentages ikke' },
  { value: 'Daily', label: 'Hver dag' },
  { value: 'Weekdays', label: 'Hverdage (man–fre)' },
  { value: 'Weekly', label: 'Hver uge' },
  { value: 'Biweekly', label: 'Hver 2. uge' },
  { value: 'Monthly', label: 'Hver måned' },
]

/** Standard-slutdato for en ny serie: 4 uger efter startdatoen. */
function defaultRepeatUntil(date: string): string {
  const until = fromDateAndTime(date, '00:00')
  until.setDate(until.getDate() + 28)
  return toDateInputValue(until)
}

/** Enten en ny opgave i et valgt tidsrum, eller en eksisterende opgave der skal redigeres. */
export type TaskModalMode = { kind: 'create'; start: Date; end: Date } | { kind: 'edit'; task: TaskItem }

interface TaskModalProps {
  mode: TaskModalMode
  onClose: () => void
  /** Kaldes efter opret, ret eller slet, så kalenderen kan hente opgaverne igen. */
  onChanged: () => void
  onCompleted: (result: CompleteTaskResponse) => void
}

export default function TaskModal({ mode, onClose, onChanged, onCompleted }: TaskModalProps) {
  const titleId = useId()
  const task = mode.kind === 'edit' ? mode.task : null
  const initialStart = mode.kind === 'edit' ? fromLocalIso(mode.task.scheduledStart) : mode.start
  const initialEnd =
    mode.kind === 'edit'
      ? mode.task.scheduledEnd
        ? fromLocalIso(mode.task.scheduledEnd)
        : null
      : mode.end

  const [title, setTitle] = useState(task?.title ?? '')
  const [description, setDescription] = useState(task?.description ?? '')
  const [date, setDate] = useState(toDateInputValue(initialStart))
  const [startTime, setStartTime] = useState(toTimeInputValue(initialStart))
  const [endTime, setEndTime] = useState(initialEnd ? toTimeInputValue(initialEnd) : '')
  const [repeat, setRepeat] = useState<RecurrenceFrequency | ''>('')
  const [repeatUntil, setRepeatUntil] = useState('')
  const [fieldErrors, setFieldErrors] = useState<Record<string, string>>({})
  const [error, setError] = useState<string | null>(null)
  const [busy, setBusy] = useState<'save' | 'delete' | 'complete' | null>(null)
  const [confirmDelete, setConfirmDelete] = useState(false)

  const isCompleted = task?.status === 'Completed'

  // Luk med Escape og lås scroll på siden bag modalen
  useEffect(() => {
    const onKeyDown = (event: KeyboardEvent) => event.key === 'Escape' && onClose()
    document.addEventListener('keydown', onKeyDown)
    document.body.classList.add('modal-open')
    return () => {
      document.removeEventListener('keydown', onKeyDown)
      document.body.classList.remove('modal-open')
    }
  }, [onClose])

  async function handleSave(event: FormEvent) {
    event.preventDefault()
    setError(null)
    setFieldErrors({})
    setBusy('save')

    const request = {
      title,
      description: description.trim() || null,
      scheduledStart: toLocalIso(fromDateAndTime(date, startTime)),
      scheduledEnd: endTime ? toLocalIso(fromDateAndTime(date, endTime)) : null,
    }

    try {
      if (task) {
        await updateTask(task.id, request)
      } else {
        await createTask(repeat ? { ...request, repeat, repeatUntil } : request)
      }
      onChanged()
      onClose()
    } catch (err) {
      const errors = getFieldErrors(err)
      setFieldErrors(errors)
      if (Object.keys(errors).length === 0) {
        setError(getErrorMessage(err))
      }
    } finally {
      setBusy(null)
    }
  }

  /** following = også senere, ikke-gennemførte opgaver i samme serie. */
  async function handleDelete(following: boolean) {
    if (!task) return
    setError(null)
    setBusy('delete')

    try {
      await deleteTask(task.id, following)
      onChanged()
      onClose()
    } catch (err) {
      setError(getErrorMessage(err))
      setBusy(null)
    }
  }

  async function handleComplete() {
    if (!task) return
    setError(null)
    setBusy('complete')

    try {
      onCompleted(await completeTask(task.id))
      onChanged()
      onClose()
    } catch (err) {
      setError(getErrorMessage(err))
      setBusy(null)
    }
  }

  return (
    <>
      <div className="modal d-block" tabIndex={-1} role="dialog" aria-modal="true" aria-labelledby={titleId}>
        <div className="modal-dialog modal-dialog-centered">
          <form className="modal-content" onSubmit={handleSave} noValidate>
            <div className="modal-header">
              <h2 id={titleId} className="modal-title h5">
                {task ? 'Rediger opgave' : 'Ny opgave'}
                {isCompleted && <span className="badge text-bg-success ms-2 align-middle fs-6">Gennemført</span>}
              </h2>
              <button type="button" className="btn-close" aria-label="Luk" onClick={onClose} />
            </div>

            <div className="modal-body">
              {error && <Alert>{error}</Alert>}

              <TextField
                label="Titel"
                value={title}
                onChange={(e) => setTitle(e.target.value)}
                error={fieldErrors.title}
                maxLength={150}
                required
                autoFocus
              />
              <TextAreaField
                label="Beskrivelse"
                rows={3}
                value={description}
                onChange={(e) => setDescription(e.target.value)}
                error={fieldErrors.description}
                maxLength={2000}
              />

              <div className="row g-2">
                <div className="col-12 col-sm-6">
                  <TextField
                    label="Dato"
                    type="date"
                    value={date}
                    onChange={(e) => setDate(e.target.value)}
                    error={fieldErrors.scheduledStart}
                    required
                  />
                </div>
                <div className="col-6 col-sm-3">
                  <TextField
                    label="Fra"
                    type="time"
                    value={startTime}
                    onChange={(e) => setStartTime(e.target.value)}
                    required
                  />
                </div>
                <div className="col-6 col-sm-3">
                  <TextField
                    label="Til"
                    type="time"
                    value={endTime}
                    onChange={(e) => setEndTime(e.target.value)}
                    error={fieldErrors.scheduledEnd}
                  />
                </div>
              </div>

              {/* Gentagelse kan kun vælges ved oprettelse – hver gentagelse bliver sin egen opgave */}
              {!task && (
                <div className="row g-2">
                  <div className="col-12 col-sm-6">
                    <SelectField
                      label="Gentag"
                      value={repeat}
                      options={repeatOptions}
                      onChange={(e) => {
                        const value = e.target.value as RecurrenceFrequency | ''
                        setRepeat(value)
                        if (value && !repeatUntil) setRepeatUntil(defaultRepeatUntil(date))
                      }}
                    />
                  </div>
                  {repeat && (
                    <div className="col-12 col-sm-6">
                      <TextField
                        label="Gentag indtil"
                        type="date"
                        value={repeatUntil}
                        min={date}
                        onChange={(e) => setRepeatUntil(e.target.value)}
                        error={fieldErrors.repeatUntil}
                        required
                      />
                    </div>
                  )}
                </div>
              )}

              {task?.seriesId && (
                <p className="small text-body-secondary mb-0">
                  ↻ Del af en gentagende serie. Ændringer gælder kun denne opgave.
                </p>
              )}
            </div>

            <div className="modal-footer">
              {task && !confirmDelete && (
                <Button
                  type="button"
                  variant="outline-danger"
                  className="me-auto"
                  onClick={() => setConfirmDelete(true)}
                  disabled={busy !== null}
                >
                  Slet
                </Button>
              )}
              {task && confirmDelete && (
                <div className="me-auto d-flex flex-wrap align-items-center gap-2">
                  <span className="small">Slet opgaven?</span>
                  {task.seriesId ? (
                    <>
                      <Button type="button" variant="danger" className="btn-sm" onClick={() => handleDelete(false)} loading={busy === 'delete'}>
                        Kun denne
                      </Button>
                      <Button type="button" variant="danger" className="btn-sm" onClick={() => handleDelete(true)} disabled={busy !== null}>
                        Denne og fremtidige
                      </Button>
                    </>
                  ) : (
                    <Button type="button" variant="danger" onClick={() => handleDelete(false)} loading={busy === 'delete'}>
                      Ja, slet
                    </Button>
                  )}
                  <Button type="button" variant="link" onClick={() => setConfirmDelete(false)}>
                    Nej
                  </Button>
                </div>
              )}

              {task && !isCompleted && (
                <Button
                  type="button"
                  variant="success"
                  onClick={handleComplete}
                  loading={busy === 'complete'}
                  disabled={busy !== null}
                >
                  Gennemfør
                </Button>
              )}
              <Button type="submit" loading={busy === 'save'} disabled={busy !== null}>
                {task ? 'Gem' : 'Opret'}
              </Button>
            </div>
          </form>
        </div>
      </div>
      <div className="modal-backdrop show" onClick={onClose} />
    </>
  )
}
