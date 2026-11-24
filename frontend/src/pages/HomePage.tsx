import { useEffect, useState } from 'react'
import { api } from '../api/client'

type ApiStatus = 'checking' | 'online' | 'offline'

export default function HomePage() {
  const [status, setStatus] = useState<ApiStatus>('checking')

  useEffect(() => {
    api
      .get('/health')
      .then(() => setStatus('online'))
      .catch(() => setStatus('offline'))
  }, [])

  return (
    <main>
      <h1>LevelUp</h1>
      <p>API status: {status}</p>
    </main>
  )
}
