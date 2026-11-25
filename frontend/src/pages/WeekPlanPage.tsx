const days = ['Mandag', 'Tirsdag', 'Onsdag', 'Torsdag', 'Fredag', 'Lørdag', 'Søndag']

export default function WeekPlanPage() {
  return (
    <div className="week">
      {days.map((day) => (
        <section key={day} className="card week__day">
          <h2>{day}</h2>
          <p className="muted">Ingen opgaver</p>
        </section>
      ))}
    </div>
  )
}
