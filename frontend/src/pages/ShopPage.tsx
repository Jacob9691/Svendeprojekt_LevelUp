import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import { getErrorMessage } from '../api/errors'
import { getShopItems, purchaseItem } from '../api/shop'
import type { ShopItem } from '../api/types'
import { useAuth } from '../auth/useAuth'
import { Alert, Button } from '../components/Form'
import { categoryInfo } from '../utils/categories'
import { achievementsMessage } from '../utils/rewards'

/** Grupperer genstandene efter kategori i den rækkefølge API'et sender dem. */
function groupByCategory(items: ShopItem[]): [string, ShopItem[]][] {
  const groups = new Map<string, ShopItem[]>()
  for (const item of items) {
    groups.set(item.category, [...(groups.get(item.category) ?? []), item])
  }
  return [...groups.entries()]
}

export default function ShopPage() {
  const { profile, refreshProfile } = useAuth()
  const [items, setItems] = useState<ShopItem[] | null>(null)
  const [confirmId, setConfirmId] = useState<number | null>(null)
  const [buyingId, setBuyingId] = useState<number | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [success, setSuccess] = useState<string | null>(null)

  useEffect(() => {
    getShopItems()
      .then(setItems)
      .catch((err) => setError(getErrorMessage(err)))
  }, [])

  const coins = profile?.coinBalance ?? 0

  async function handleBuy(item: ShopItem) {
    setError(null)
    setSuccess(null)
    setBuyingId(item.id)

    try {
      const result = await purchaseItem(item.id)
      setItems((current) => current?.map((i) => (i.id === item.id ? { ...i, owned: true } : i)) ?? null)
      setSuccess(`Du har købt ${item.name}! ${achievementsMessage(result.unlockedAchievements)}`)
      await refreshProfile()
    } catch (err) {
      setError(getErrorMessage(err))
    } finally {
      setBuyingId(null)
      setConfirmId(null)
    }
  }

  if (!items) {
    return error ? <Alert>{error}</Alert> : <p className="text-body-secondary">Henter shoppen…</p>
  }

  return (
    <>
      <p className="text-body-secondary">
        Du har <span className="badge text-bg-warning">{coins} points</span>. Gennemfør opgaver i ugeplanen for at
        optjene flere.
      </p>

      {error && <Alert>{error}</Alert>}
      {success && (
        <div className="alert alert-success alert-dismissible py-2" role="status">
          {success} Udstyr den på <Link to="/karakter" className="alert-link">Karakter</Link>-siden.
          <button type="button" className="btn-close py-2" aria-label="Luk" onClick={() => setSuccess(null)} />
        </div>
      )}

      {groupByCategory(items).map(([category, categoryItems]) => {
        const { label, icon } = categoryInfo(category)

        return (
          <section key={category} className="mb-4">
            <h2 className="h5 mb-3">{label}</h2>
            <div className="row g-3">
              {categoryItems.map((item) => {
                const missing = item.price - coins
                const isConfirming = confirmId === item.id

                return (
                  <div key={item.id} className="col-12 col-sm-6 col-lg-4">
                    <div className={`card h-100${item.owned ? ' border-success' : ''}`}>
                      <div className="card-body d-flex gap-3">
                        <div className="fs-1 lh-1" aria-hidden="true">
                          {icon}
                        </div>
                        <div>
                          <h3 className="h6 card-title mb-1">{item.name}</h3>
                          <p className="card-text small text-body-secondary mb-2">{item.description}</p>
                          <span className="badge text-bg-warning">{item.price} points</span>
                        </div>
                      </div>

                      <div className="card-footer bg-transparent">
                        {item.owned ? (
                          <span className="text-success small fw-semibold">✓ Købt</span>
                        ) : isConfirming ? (
                          <div className="d-flex align-items-center gap-2">
                            <span className="small me-auto">Køb for {item.price} points?</span>
                            <Button
                              className="btn-sm"
                              onClick={() => handleBuy(item)}
                              loading={buyingId === item.id}
                            >
                              Ja, køb
                            </Button>
                            <Button
                              className="btn-sm"
                              variant="link"
                              onClick={() => setConfirmId(null)}
                              disabled={buyingId === item.id}
                            >
                              Nej
                            </Button>
                          </div>
                        ) : missing > 0 ? (
                          <span className="small text-body-secondary">Du mangler {missing} points</span>
                        ) : (
                          <Button className="btn-sm" onClick={() => setConfirmId(item.id)} disabled={buyingId !== null}>
                            Køb
                          </Button>
                        )}
                      </div>
                    </div>
                  </div>
                )
              })}
            </div>
          </section>
        )
      })}
    </>
  )
}
