import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getErrorMessage } from '../api/errors'
import { equipItem, getInventory, unequipItem } from '../api/inventory'
import type { InventoryItem } from '../api/types'
import { useAuth } from '../auth/useAuth'
import { Alert, Button } from '../components/Form'
import { categories, categoryInfo } from '../utils/categories'

export default function CharacterPage() {
  const { profile } = useAuth()
  const [inventory, setInventory] = useState<InventoryItem[] | null>(null)
  const [busyId, setBusyId] = useState<number | null>(null)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    getInventory()
      .then(setInventory)
      .catch((err) => setError(getErrorMessage(err)))
  }, [])

  async function toggleEquipped(item: InventoryItem) {
    setError(null)
    setBusyId(item.id)

    try {
      await (item.isEquipped ? unequipItem(item.id) : equipItem(item.id))
      // Hent hele inventaret igen – backend tager andre genstande i samme kategori af
      setInventory(await getInventory())
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setBusyId(null)
    }
  }

  if (!inventory) {
    return error ? <Alert>{error}</Alert> : <p className="text-body-secondary">Henter din karakter…</p>
  }

  const equipped = inventory.filter((item) => item.isEquipped)

  return (
    <div className="row g-3">
      <div className="col-lg-4">
        <div className="card h-100">
          <div className="card-body text-center">
            <div className="display-1" aria-hidden="true">
              🧙
            </div>
            <h2 className="h5 mb-1">{profile?.displayName}</h2>
            {profile && <span className="badge text-bg-primary">Level {profile.level}</span>}
          </div>

          <ul className="list-group list-group-flush">
            {Object.entries(categories).map(([category, info]) => {
              const item = equipped.find((i) => i.category === category)

              return (
                <li key={category} className="list-group-item d-flex justify-content-between align-items-center">
                  <span className="text-body-secondary">{info.slot}</span>
                  {item ? (
                    <span>
                      <span aria-hidden="true">{info.icon}</span> {item.name}
                    </span>
                  ) : (
                    <span className="text-body-secondary small">Intet</span>
                  )}
                </li>
              )
            })}
          </ul>
        </div>
      </div>

      <div className="col-lg-8">
        <div className="card h-100">
          <div className="card-header fw-semibold">Inventar</div>

          {error && (
            <div className="card-body pb-0">
              <Alert>{error}</Alert>
            </div>
          )}

          {inventory.length === 0 ? (
            <div className="card-body">
              <p className="text-body-secondary mb-2">Du har ingen genstande endnu.</p>
              <Link to="/shop" className="btn btn-primary btn-sm">
                Gå til shoppen
              </Link>
            </div>
          ) : (
            <ul className="list-group list-group-flush">
              {inventory.map((item) => {
                const info = categoryInfo(item.category)

                return (
                  <li key={item.id} className="list-group-item d-flex align-items-center gap-3">
                    <span className="fs-3" aria-hidden="true">
                      {info.icon}
                    </span>
                    <div className="me-auto">
                      <div className="fw-semibold">
                        {item.name}
                        {item.isEquipped && <span className="badge text-bg-success ms-2">Udstyret</span>}
                      </div>
                      <div className="small text-body-secondary">{info.slot}</div>
                    </div>
                    <Button
                      className="btn-sm text-nowrap"
                      variant={item.isEquipped ? 'outline-secondary' : 'primary'}
                      onClick={() => toggleEquipped(item)}
                      loading={busyId === item.id}
                      disabled={busyId !== null}
                    >
                      {item.isEquipped ? 'Tag af' : 'Udstyr'}
                    </Button>
                  </li>
                )
              })}
            </ul>
          )}
        </div>
      </div>
    </div>
  )
}
