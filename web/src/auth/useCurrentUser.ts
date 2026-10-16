import { useQuery } from "@tanstack/react-query"
import { fetchMe } from "../api/auth"

export const currentUserKey = ['me']

export function useCurrentUser(){
    return useQuery({
        queryKey: currentUserKey,
        queryFn: fetchMe,
        retry: false,
        staleTime: 5 * 60_000
    })
}