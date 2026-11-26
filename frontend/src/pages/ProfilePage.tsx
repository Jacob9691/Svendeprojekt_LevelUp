import { useAuth } from '../auth/useAuth'
import { Button } from '../components/Form'

export default function ProfilePage() {
  const { profile, signOut } = useAuth()

  if (!profile) {
    return <p className="text-body-secondary">Henter profil…</p>
  }

  return (
    <div className="row g-3">
      <div className="col-lg-6">
        <div className="card h-100">
          <div className="card-body">
            <h2 className="h6 card-title mb-3">Personlige oplysninger</h2>
            <dl className="row mb-0">
              <dt className="col-sm-5 fw-normal text-body-secondary">Navn</dt>
              <dd className="col-sm-7">{profile.displayName}</dd>
              <dt className="col-sm-5 fw-normal text-body-secondary">E-mail</dt>
              <dd className="col-sm-7">{profile.email}</dd>
              <dt className="col-sm-5 fw-normal text-body-secondary">Medlem siden</dt>
              <dd className="col-sm-7 mb-0">{new Date(profile.createdAt).toLocaleDateString('da-DK')}</dd>
            </dl>
          </div>
        </div>
      </div>

      <div className="col-lg-6">
        <div className="card h-100">
          <div className="card-body">
            <h2 className="h6 card-title mb-3">Fremskridt</h2>
            <dl className="row mb-0">
              <dt className="col-sm-5 fw-normal text-body-secondary">Level</dt>
              <dd className="col-sm-7">{profile.level}</dd>
              <dt className="col-sm-5 fw-normal text-body-secondary">XP</dt>
              <dd className="col-sm-7">
                {profile.totalXp} ({profile.xpIntoLevel}/{profile.xpForNextLevel} til næste level)
              </dd>
              <dt className="col-sm-5 fw-normal text-body-secondary">Points</dt>
              <dd className="col-sm-7">{profile.coinBalance}</dd>
              <dt className="col-sm-5 fw-normal text-body-secondary">Gennemførte opgaver</dt>
              <dd className="col-sm-7">{profile.completedTasks}</dd>
              <dt className="col-sm-5 fw-normal text-body-secondary">Genstande</dt>
              <dd className="col-sm-7 mb-0">{profile.inventoryCount}</dd>
            </dl>
          </div>
        </div>
      </div>

      <div className="col-12">
        <div className="card">
          <div className="card-body">
            <h2 className="h6 card-title">Konto</h2>
            <p className="card-text text-body-secondary">Indstillinger for påmindelser og 2FA kommer her.</p>
            <Button variant="outline-secondary" onClick={signOut}>
              Log ud
            </Button>
          </div>
        </div>
      </div>
    </div>
  )
}
