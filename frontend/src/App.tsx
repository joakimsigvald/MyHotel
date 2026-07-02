import { useCallback, useEffect, useState } from 'react'
import { cancelReservation, getReservations, getRooms, makeReservation } from './api'
import type { Reservation, Room } from './types'
import './App.css'

const emptyForm = {
  roomId: '',
  guestName: '',
  guestEmail: '',
  checkIn: '',
  checkOut: '',
}

function App() {
  const [rooms, setRooms] = useState<Room[]>([])
  const [reservations, setReservations] = useState<Reservation[]>([])
  const [form, setForm] = useState(emptyForm)
  const [error, setError] = useState('')
  const [busy, setBusy] = useState(false)

  const refresh = useCallback(async () => {
    try {
      const [roomList, reservationList] = await Promise.all([getRooms(), getReservations()])
      setRooms(roomList)
      setReservations(reservationList)
    } catch (e) {
      setError(e instanceof Error ? e.message : 'Failed to load data')
    }
  }, [])

  useEffect(() => {
    void refresh()
  }, [refresh])

  const submit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError('')
    setBusy(true)
    try {
      await makeReservation(form)
      setForm(emptyForm)
      await refresh()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Booking failed')
    } finally {
      setBusy(false)
    }
  }

  const cancel = async (id: string) => {
    setError('')
    try {
      await cancelReservation(id)
      await refresh()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Cancellation failed')
    }
  }

  const roomLabel = (roomId: string) => {
    const room = rooms.find((r) => r.id === roomId)
    return room ? `${room.number} — ${room.name}` : roomId
  }

  return (
    <main>
      <h1>MyHotel</h1>

      <section>
        <h2>Rooms</h2>
        <table>
          <thead>
            <tr>
              <th>Number</th>
              <th>Name</th>
              <th>Capacity</th>
              <th>Price / night</th>
            </tr>
          </thead>
          <tbody>
            {rooms.map((room) => (
              <tr key={room.id}>
                <td>{room.number}</td>
                <td>{room.name}</td>
                <td>{room.capacity}</td>
                <td>€{room.pricePerNight}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </section>

      <section>
        <h2>New reservation</h2>
        <form onSubmit={submit}>
          <label>
            Room
            <select
              required
              value={form.roomId}
              onChange={(e) => setForm({ ...form, roomId: e.target.value })}
            >
              <option value="">Select a room…</option>
              {rooms.map((room) => (
                <option key={room.id} value={room.id}>
                  {room.number} — {room.name} (sleeps {room.capacity})
                </option>
              ))}
            </select>
          </label>
          <label>
            Guest name
            <input
              required
              value={form.guestName}
              onChange={(e) => setForm({ ...form, guestName: e.target.value })}
            />
          </label>
          <label>
            Guest email
            <input
              type="email"
              value={form.guestEmail}
              onChange={(e) => setForm({ ...form, guestEmail: e.target.value })}
            />
          </label>
          <label>
            Check-in
            <input
              type="date"
              required
              value={form.checkIn}
              onChange={(e) => setForm({ ...form, checkIn: e.target.value })}
            />
          </label>
          <label>
            Check-out
            <input
              type="date"
              required
              value={form.checkOut}
              onChange={(e) => setForm({ ...form, checkOut: e.target.value })}
            />
          </label>
          <button type="submit" disabled={busy}>
            {busy ? 'Booking…' : 'Book'}
          </button>
        </form>
        {error && <p className="error">{error}</p>}
      </section>

      <section>
        <h2>Reservations</h2>
        {reservations.length === 0 ? (
          <p>No reservations yet.</p>
        ) : (
          <table>
            <thead>
              <tr>
                <th>Room</th>
                <th>Guest</th>
                <th>Check-in</th>
                <th>Check-out</th>
                <th>Status</th>
                <th></th>
              </tr>
            </thead>
            <tbody>
              {reservations.map((reservation) => (
                <tr key={reservation.id} className={reservation.status === 'Cancelled' ? 'cancelled' : ''}>
                  <td>{roomLabel(reservation.roomId)}</td>
                  <td>{reservation.guestName}</td>
                  <td>{reservation.checkIn}</td>
                  <td>{reservation.checkOut}</td>
                  <td>{reservation.status}</td>
                  <td>
                    {reservation.status === 'Confirmed' && (
                      <button type="button" onClick={() => cancel(reservation.id)}>
                        Cancel
                      </button>
                    )}
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </section>
    </main>
  )
}

export default App
