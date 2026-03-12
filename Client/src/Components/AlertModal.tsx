import type { Alert } from "../generated-ts-client.ts";

interface AlertModalProps {
    alerts: Alert[];
}

function AlertModal({ alerts }: AlertModalProps) {
    const getSeverityColor = (severity?: string) => {
        switch (severity?.toLowerCase()) {
            case 'critical':
                return 'badge-error';
            case 'warning':
                return 'badge-warning';
            case 'info':
                return 'badge-info';
            default:
                return 'badge-neutral';
        }
    };

    const formatTimestamp = (timestamp?: string) => {
        if (!timestamp) return 'N/A';
        return new Date(timestamp).toLocaleString();
    };

    return (
        <dialog id="alert_modal" className="modal">
            <div className="modal-box w-11/12 max-w-5xl">
                <h3 className="font-bold text-lg mb-4">Alerts ({alerts.length})</h3>
                
                {alerts.length === 0 ? (
                    <p className="py-4 text-gray-500">No alerts to display</p>
                ) : (
                    <div className="overflow-x-auto max-h-96">
                        <table className="table table-zebra">
                            <thead>
                                <tr>
                                    <th>Timestamp</th>
                                    <th>Turbine</th>
                                    <th>Severity</th>
                                    <th>Message</th>
                                </tr>
                            </thead>
                            <tbody>
                                {alerts.slice().reverse().map((alert, index) => (
                                    <tr key={alert.id || index}>
                                        <td className="text-sm">{formatTimestamp(alert.timestamp)}</td>
                                        <td className="font-medium">{alert.turbineid || 'N/A'}</td>
                                        <td>
                                            <span className={`badge ${getSeverityColor(alert.severity)}`}>
                                                {alert.severity || 'Unknown'}
                                            </span>
                                        </td>
                                        <td>{alert.message || 'No message'}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
                
                <div className="modal-action">
                    <form method="dialog">
                        <button className="btn">Close</button>
                    </form>
                </div>
            </div>
        </dialog>
    );
}

export default AlertModal;