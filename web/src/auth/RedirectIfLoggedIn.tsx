import {Navigate, Outlet } from "react-router";
import { useCurrentUser } from "./useCurrentUser";

export default function RequireAuth(){
    const {data: user, isPending} = useCurrentUser();


    if(isPending){
        return(
            <div className="flex min-h-screen items-center justify-center">
                <span className="loading loading-spinner loading-lg" />
            </div>
        )
    }

if(user){

    return <Navigate to="/" replace />
}
return <Outlet/>
}