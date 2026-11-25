const year = new Date().getFullYear()

export default function Footer() {
  return (
    <footer className="footer">
      <div className="container footer__inner">
        <span>
          © {year} Level<span className="accent">Up</span>
        </span>
        <span className="muted">Gamificeret planlægning</span>
      </div>
    </footer>
  )
}
