import { useEffect, useState } from 'react'
import { api } from '../api/client'

type ApiStatus = 'tjekker…' | 'online' | 'offline'

export default function DashboardPage() {
  const [status, setStatus] = useState<ApiStatus>('tjekker…')

  useEffect(() => {
    api
      .get('/health')
      .then(() => setStatus('online'))
      .catch(() => setStatus('offline'))
  }, [])

  return (
    <div className="grid">
      <section className="card">
        <h2>Dagens opgaver</h2>
        <p className="muted">Her vises dagens opgaver.</p>
      </section>
      <section className="card">
        <h2>Fremskridt</h2>
        <p className="muted">Her vises XP, level og achievements.</p>
      </section>
      <section className="card">
        <h2>System</h2>
        <p className="muted">API status: {status}</p>
      </section>
    </div>
  )
}
