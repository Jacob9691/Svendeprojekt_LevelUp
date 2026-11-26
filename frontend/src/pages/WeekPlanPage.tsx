import FullCalendar, {
  type CalendarRef,
  type DateSelectInfo,
  type EventClickInfo,
  type EventDropInfo,
  type EventInput,
  type EventResizeDoneInfo,
  type EventSourceFuncInfo,
} from '@fullcalendar/react'
import interactionPlugin from '@fullcalendar/react/interaction'
import daLocale from '@fullcalendar/react/locales/da'
import '@fullcalendar/react/skeleton.css'
import classicThemePlugin from '@fullcalendar/react/themes/classic'
import '@fullcalendar/react/themes/classic/palette.css'
import '@fullcalendar/react/themes/classic/theme.css'
import timeGridPlugin from '@fullcalendar/react/timegrid'
import { useCallback, useRef, useState } from 'react'
import { getErrorMessage } from '../api/errors'
import { getTasks, updateTask } from '../api/tasks'
import type { CompleteTaskResponse, TaskItem } from '../api/types'
import { useAuth } from '../auth/useAuth'
import { Alert } from '../components/Form'
import TaskModal, { type TaskModalMode } from '../components/TaskModal'
import { addMinutes, toLocalIso } from '../utils/dates'
import { rewardMessage } from '../utils/rewards'

const TASK_COLOR = '#0d6efd' // Bootstrap "primary"
const COMPLETED_COLOR = '#198754' // Bootstrap "success"

function toEvent(task: TaskItem): EventInput {
  const completed = task.status === 'Completed'

  // Felter sættes kun når de har en værdi – FullCalendar opfatter "color: undefined" som en farve
  return {
    id: String(task.id),
    // ↻ viser at opgaven er del af en gentagende serie
    title: task.seriesId ? `↻ ${task.title}` : task.title,
    start: task.scheduledStart,
    ...(task.scheduledEnd && { end: task.scheduledEnd }),
    ...(completed && { color: COMPLETED_COLOR }),
    // Gennemførte opgaver kan ikke flyttes, kun åbnes
    editable: !completed,
    extendedProps: { task },
  }
}

/** Næste hele time – bruges når man trykker "Ny opgave". */
function nextFullHour(): Date {
  const date = new Date()
  date.setHours(date.getHours() + 1, 0, 0, 0)
  return date
}

export default function WeekPlanPage() {
  const calendarRef = useRef<CalendarRef>(null)
  const { refreshProfile } = useAuth()
  const [modal, setModal] = useState<TaskModalMode | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [reward, setReward] = useState<string | null>(null)

  // Henter opgaverne for den uge/dag kalenderen viser. useCallback så kalenderen ikke henter igen ved hver render.
  const loadEvents = useCallback(async (info: EventSourceFuncInfo): Promise<EventInput[]> => {
    try {
      const tasks = await getTasks(toLocalIso(info.start), toLocalIso(info.end))
      setError(null)
      return tasks.map(toEvent)
    } catch (err) {
      setError(getErrorMessage(err))
      return []
    }
  }, [])

  const refetch = useCallback(() => calendarRef.current?.getApi().refetchEvents(), [])
  const closeModal = useCallback(() => setModal(null), [])

  function handleSelect(info: DateSelectInfo) {
    setModal({ kind: 'create', start: info.start, end: info.end })
    calendarRef.current?.getApi().unselect()
  }

  function handleEventClick(info: EventClickInfo) {
    setModal({ kind: 'edit', task: info.event.extendedProps.task as TaskItem })
  }

  /** Opgaven er trukket til et nyt tidspunkt eller gjort længere/kortere. */
  async function handleMove(info: EventDropInfo | EventResizeDoneInfo) {
    const task = info.event.extendedProps.task as TaskItem
    const { start, end } = info.event
    if (!start) return

    try {
      await updateTask(task.id, {
        title: task.title,
        description: task.description,
        scheduledStart: toLocalIso(start),
        scheduledEnd: end ? toLocalIso(end) : null,
      })
      refetch()
    } catch (err) {
      info.revert()
      setError(getErrorMessage(err))
    }
  }

  function handleCompleted(result: CompleteTaskResponse) {
    setReward(rewardMessage(result))
    void refreshProfile()
  }

  function openNewTask() {
    const start = nextFullHour()
    setModal({ kind: 'create', start, end: addMinutes(start, 60) })
  }

  return (
    <>
      <div className="d-flex justify-content-between align-items-center gap-3 mb-3">
        <p className="text-body-secondary mb-0">Træk i kalenderen eller tryk på en opgave for at redigere den.</p>
        <button type="button" className="btn btn-primary text-nowrap" onClick={openNewTask}>
          Ny opgave
        </button>
      </div>

      {error && <Alert>{error}</Alert>}
      {reward && (
        <div className="alert alert-success alert-dismissible py-2" role="status">
          {reward}
          <button type="button" className="btn-close py-2" aria-label="Luk" onClick={() => setReward(null)} />
        </div>
      )}

      <div className="card">
        <div className="card-body">
          <FullCalendar
            ref={calendarRef}
            plugins={[timeGridPlugin, interactionPlugin, classicThemePlugin]}
            locale={daLocale}
            initialView={window.innerWidth < 768 ? 'timeGridDay' : 'timeGridWeek'}
            headerToolbar={{ start: 'prev,next today', center: 'title', end: 'timeGridWeek,timeGridDay' }}
            allDaySlot={false}
            slotMinTime="06:00"
            slotMaxTime="24:00"
            scrollTime="08:00"
            height="70vh"
            eventColor={TASK_COLOR}
            nowIndicator
            selectable
            selectMirror
            editable
            events={loadEvents}
            select={handleSelect}
            eventClick={handleEventClick}
            eventDrop={handleMove}
            eventResize={handleMove}
          />
        </div>
      </div>

      {modal && (
        <TaskModal mode={modal} onClose={closeModal} onChanged={refetch} onCompleted={handleCompleted} />
      )}
    </>
  )
}
