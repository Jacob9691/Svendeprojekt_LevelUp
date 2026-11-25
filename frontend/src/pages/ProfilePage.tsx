import { useAuth } from '../auth/useAuth'
import { Button } from '../components/Form'

export default function ProfilePage() {
  const { profile, signOut } = useAuth()

  if (!profile) {
    return <p className="muted">Henter profil…</p>
  }

  return (
    <div className="grid">
      <section className="card">
        <h2>Personlige oplysninger</h2>
        <dl className="details">
          <dt>Navn</dt>
          <dd>{profile.displayName}</dd>
          <dt>E-mail</dt>
          <dd>{profile.email}</dd>
          <dt>Medlem siden</dt>
          <dd>{new Date(profile.createdAt).toLocaleDateString('da-DK')}</dd>
        </dl>
      </section>

      <section className="card">
        <h2>Fremskridt</h2>
        <dl className="details">
          <dt>Level</dt>
          <dd>{profile.level}</dd>
          <dt>XP</dt>
          <dd>
            {profile.totalXp} ({profile.xpIntoLevel}/{profile.xpForNextLevel} til næste level)
          </dd>
          <dt>Points</dt>
          <dd>{profile.coinBalance}</dd>
          <dt>Gennemførte opgaver</dt>
          <dd>{profile.completedTasks}</dd>
          <dt>Genstande</dt>
          <dd>{profile.inventoryCount}</dd>
        </dl>
      </section>

      <section className="card">
        <h2>Konto</h2>
        <p className="muted">Indstillinger for påmindelser og 2FA kommer her.</p>
        <Button variant="secondary" onClick={signOut}>
          Log ud
        </Button>
      </section>
    </div>
  )
}
