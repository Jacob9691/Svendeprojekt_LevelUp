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
    <div className="row g-3">
      <div className="col-md-4">
        <div className="card h-100">
          <div className="card-body">
            <h2 className="h6 card-title">Dagens opgaver</h2>
            <p className="card-text text-body-secondary">Her vises dagens opgaver.</p>
          </div>
        </div>
      </div>
      <div className="col-md-4">
        <div className="card h-100">
          <div className="card-body">
            <h2 className="h6 card-title">Fremskridt</h2>
            <p className="card-text text-body-secondary">Her vises XP, level og achievements.</p>
          </div>
        </div>
      </div>
      <div className="col-md-4">
        <div className="card h-100">
          <div className="card-body">
            <h2 className="h6 card-title">System</h2>
            <p className="card-text text-body-secondary">API status: {status}</p>
          </div>
        </div>
      </div>
    </div>
  )
}
