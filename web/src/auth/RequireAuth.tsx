import {Navigate, Outlet, useLocation } from "react-router";
import { useCurrentUser } from "./useCurrentUser";

export default function RequireAuth(){
    const {data: user, isPending} = useCurrentUser();
    const location = useLocation();


    if(isPending){
        return(
            <div className="flex min-h-screen items-center justify-center">
                <span className="loading loading-spinner loading-lg" />
            </div>
        )
    }

if(!user){

    return <Navigate to="/login" replace state={{from: location.pathname}}/>
}
return <Outlet/>
}