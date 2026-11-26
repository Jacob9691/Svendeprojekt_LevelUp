const days = ['Mandag', 'Tirsdag', 'Onsdag', 'Torsdag', 'Fredag', 'Lørdag', 'Søndag']

export default function WeekPlanPage() {
  return (
    <div className="row g-3">
      {days.map((day) => (
        // 1 kolonne på mobil, 2 på tablet, 4 på laptop og alle 7 på store skærme
        <div key={day} className="col-12 col-sm-6 col-lg-3 col-xxl">
          <div className="card h-100">
            <div className="card-header fw-semibold">{day}</div>
            <div className="card-body">
              <p className="card-text text-body-secondary small">Ingen opgaver</p>
            </div>
          </div>
        </div>
      ))}
    </div>
  )
}
