import type { MakeReservation, Reservation, Room } from './types'

async function throwApiError(response: Response): Promise<never> {
  let message = `Request failed (${response.status})`
  try {
    const body = await response.json()
    if (typeof body === 'string') {
      message = body
    } else if (body?.errors) {
      // ASP.NET ValidationProblem shape
      message = Object.values<string[]>(body.errors).flat().join(' ')
    } else if (body?.title) {
      message = body.title
    }
  } catch {
    // no JSON body — keep the generic message
  }
  throw new Error(message)
}

async function get<T>(url: string): Promise<T> {
  const response = await fetch(url)
  if (!response.ok) await throwApiError(response)
  return response.json()
}

async function post(url: string, body?: unknown): Promise<void> {
  const response = await fetch(url, {
    method: 'POST',
    headers: body !== undefined ? { 'Content-Type': 'application/json' } : undefined,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  })
  if (!response.ok) await throwApiError(response)
}

export const getRooms = () => get<Room[]>('/api/rooms')
export const getReservations = () => get<Reservation[]>('/api/reservations')
export const makeReservation = (command: MakeReservation) =>
  post('/api/reservations', command)
export const cancelReservation = (id: string) =>
  post(`/api/reservations/${id}/cancel`)
