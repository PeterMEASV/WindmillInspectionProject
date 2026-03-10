import TelemetryChart from "./TelemetryChart.tsx";
import type {Telemetry} from "../generated-ts-client.ts";

interface TelemetryCardProps {
    title: string;
    telemetries: Telemetry[];
    dataKey: keyof Telemetry;
    color: string;
    unit: string;
}

function TelemetryCard({ title, telemetries, dataKey, color, unit }: TelemetryCardProps) {
    const formatTimestamp = (timestamp: string | undefined) => {
        if (!timestamp) return '';
        const date = new Date(timestamp);
        return `${date.getHours()}:${date.getMinutes().toString().padStart(2, '0')}:${date.getSeconds().toString().padStart(2, '0')}`;
    };

    return (
        <div className="card w- bg-base-200 shadow-md border-2 border-base-300">
            <div className="card-body py-3 px-4">
                <h2 className="card-title text-base mb-1">{title}</h2>
                <TelemetryChart 
                    title={title}
                    data={telemetries.map(t => (t[dataKey] as number) ?? 0)}
                    labels={telemetries.map(t => formatTimestamp(t.timestamp))}
                    color={color}
                    unit={unit}
                />
            </div>
        </div>
    );
}

export default TelemetryCard;