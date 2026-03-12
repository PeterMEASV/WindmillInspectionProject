import type { Alert } from "../generated-ts-client.ts";
import { useState } from "react";
import { alertClient } from "../baseUrl.ts";

interface AlertModalProps {
    alerts: Alert[];
    onResolveAlert: (alertId: string) => void;
}

function AlertModal({ alerts, onResolveAlert }: AlertModalProps) {
    const [activeTab, setActiveTab] = useState<'unresolved' | 'resolved'>('unresolved');

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

    const handleResolveAlert = (alertId?: string) => {
        if (!alertId) return;

        console.log("Resolving alert: ", alertId);
        onResolveAlert(alertId);
        alertClient.resolveAlert(alertId);
    }

    const formatTimestamp = (timestamp?: string) => {
        if (!timestamp) return 'N/A';
        return new Date(timestamp).toLocaleString();
    };

    const displayedAlerts = activeTab === 'unresolved'
        ? alerts.filter(alert => !alert.resolved)
        : alerts.filter(alert => alert.resolved);

    return (
        <dialog id="alert_modal" className="modal" style={{ alignItems: 'flex-start', paddingTop: '1rem' }}>
            <div className="modal-box w-11/12 max-w-5xl">
                <h3 className="font-bold text-lg mb-4">
                    Alerts (Unresolved: {alerts.filter(a => !a.resolved).length} | Resolved: {alerts.filter(a => a.resolved).length})
                </h3>

                {/* Tab buttons */}
                <div className="flex w-full mb-4">
                    <button
                        className={`btn flex-1 rounded-r-none ${activeTab === 'unresolved' ? 'btn-primary' : 'btn-outline'}`}
                        onClick={() => setActiveTab('unresolved')}>
                        Unresolved ({alerts.filter(a => !a.resolved).length})</button>
                    <button
                        className={`btn flex-1 rounded-l-none ${activeTab === 'resolved' ? 'btn-primary' : 'btn-outline'}`}
                        onClick={() => setActiveTab('resolved')}>
                        Resolved ({alerts.filter(a => a.resolved).length})</button>
                </div>

                {displayedAlerts.length === 0 ? (
                    <p className="py-4 text-gray-500">
                        No {activeTab} alerts to display
                    </p>
                ) : (
                    <div className="overflow-x-auto max-h-96">
                        <table className="table table-zebra">
                            <thead>
                            <tr>
                                <th>Timestamp</th>
                                <th>Turbine</th>
                                <th>Severity</th>
                                <th>Message</th>
                                {activeTab === 'unresolved' && <th>Action</th>}
                            </tr>
                            </thead>
                            <tbody>
                            {displayedAlerts.slice().reverse().map((alert, index) => (
                                <tr key={alert.id || index}>
                                    <td className="text-sm">{formatTimestamp(alert.timestamp)}</td>
                                    <td className="font-medium">{alert.turbineid || 'N/A'}</td>
                                    <td>
                                            <span className={`badge ${getSeverityColor(alert.severity)}`}>
                                                {alert.severity || 'Unknown'}
                                            </span>
                                    </td>
                                    <td>{alert.message || 'No message'}</td>
                                    {activeTab === 'unresolved' && (
                                        <td>
                                            <button className="btn btn-primary btn-xs"
                                                onClick={() => handleResolveAlert(alert.id)}>Resolve</button>
                                        </td>
                                    )}
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
