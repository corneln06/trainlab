export type currentUser = {
    id : string,
    email: string
}
export type Credentials = {
    email: string,
    password: string
}
const jsonHeaders = {"Content-Type": "application/json"}

export async function fetchMe(): Promise<currentUser | null>{
    const response = await fetch("api/me")
    if(response.status == 401) return null
    if(!response.ok) throw new Error(`Unexpected status ${response.status}`)
        return response.json();
}
export async function login(credentials : Credentials): Promise<void>{
    const response = await fetch("/api/auth/login?useCookies=true",{
        method: 'POST',
        headers: jsonHeaders,
        body: JSON.stringify(credentials),
    })
    if(response.status == 401) throw new Error("Wrong password or email")
    if(!response.ok) throw new Error(`Login failed (${response.status}).`)

}
export async function register(credentials: Credentials): Promise<void> {
    const response = await fetch("api/auth/register", {
        method: 'POST',
        headers: jsonHeaders,
        body: JSON.stringify(credentials),
    })
    if(response.ok) return


    if(response.status == 400){
        const problem = (await response.json()) as {errors?: Record<string, string[]>}
        const messages = Object.values(problem.errors ?? {}).flat()
        throw new Error(messages.join(" ") || 'Registration failed')
    }
    throw new Error(`Registration failed (${response.status}).`)

}
export async function logout() : Promise<void>{
    const response = await fetch('/api/auth/logout', {method: 'POST'})
    if(!response.ok) throw new Error(`Logout failed (${response.status}).`)
}