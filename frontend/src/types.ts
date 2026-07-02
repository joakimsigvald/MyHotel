export interface Room {
  id: string
  number: string
  name: string
  capacity: number
  pricePerNight: number
}

export type ReservationStatus = 'Confirmed' | 'Cancelled'

export interface Reservation {
  id: string
  roomId: string
  guestName: string
  guestEmail: string
  checkIn: string // ISO date, e.g. "2026-07-10"
  checkOut: string
  status: ReservationStatus
}

export interface MakeReservation {
  roomId: string
  guestName: string
  guestEmail: string
  checkIn: string
  checkOut: string
}
