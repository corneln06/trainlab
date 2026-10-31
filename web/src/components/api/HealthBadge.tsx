import { useQuery } from "@tanstack/react-query";

async function fetchHealth(): Promise<string> {
    const response = await fetch("/health")

    if(response.status !== 200 && response.status !== 503){
        throw Error(`Unexpected status ${response.status}`)
    }
    return response.text();
}
export default function HealthBadge(){
    const {data, isPending, isError} = useQuery({
        queryKey: ['health'],
        queryFn: fetchHealth,
        refetchInterval: 30_000,
    })

    if(isPending) return <span className="badge badge-ghost">Checking API... </span>
    if(isError) return <span className="badge badge-error">API offline</span>

    return(
        <span className={`badge ${data === "Healthy" ? "badge-success" : "badge-warning"}`}>
            API {data}
        </span>
        
    )
}