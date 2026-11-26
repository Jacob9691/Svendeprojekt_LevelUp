const year = new Date().getFullYear()

export default function Footer() {
  return (
    <footer className="border-top py-3 mt-auto">
      <div className="container d-flex justify-content-between small text-body-secondary">
        <span>© {year} LevelUp</span>
        <span>Gamificeret planlægning</span>
      </div>
    </footer>
  )
}
