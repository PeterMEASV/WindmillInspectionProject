import * as React from 'react';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import { SparkLineChart, type SparkLineChartProps } from '@mui/x-charts/SparkLineChart';
import { areaElementClasses, lineElementClasses } from '@mui/x-charts/LineChart';
import { chartsAxisHighlightClasses } from '@mui/x-charts/ChartsAxisHighlight';
import Box from '@mui/material/Box';

interface TelemetryChartProps {
    title: string;
    data: number[];
    labels: string[];
    color: string;
    unit?: string;
}

export default function TelemetryChart({ title, data, labels, color, unit = '' }: TelemetryChartProps) {
    const [dataIndex, setDataIndex] = React.useState<null | number>(null);

    const settings: SparkLineChartProps = {
        data: data,
        baseline: 'min',
        margin: { bottom: 0, top: 5, left: 4, right: 0 },
        xAxis: { id: 'time-axis', data: labels },
        yAxis: {
            domainLimit: (minValue: number, maxValue: number) => ({
                min: minValue - (maxValue - minValue) * 0.1,
                max: maxValue + (maxValue - minValue) * 0.1,
            }),
        },
        sx: {
            [`& .${areaElementClasses.root}`]: { opacity: 0.2 },
            [`& .${lineElementClasses.root}`]: { strokeWidth: 3 },
            [`& .${chartsAxisHighlightClasses.root}`]: {
                stroke: color,
                strokeDasharray: 'none',
                strokeWidth: 2,
            },
        },
        slotProps: {
            lineHighlight: { r: 4 },
        },
        clipAreaOffset: { top: 2, bottom: 2 },
        axisHighlight: { x: 'line' },
    };

    // Show "No data" if no data points
    if (data.length === 0) {
        return (
            <Box
                width="100%"
                height="100%"
                display="flex"
                justifyContent="center"
                alignItems="center"
            >
                <Typography color="text.white">No telemetry data available</Typography>
            </Box>
        );
    }

    return (
        <Box
            onKeyDown={(event) => {
                switch (event.key) {
                    case 'ArrowLeft':
                        setDataIndex((p) =>
                            p === null ? data.length - 1 : (data.length + p - 1) % data.length,
                        );
                        break;
                    case 'ArrowRight':
                        setDataIndex((p) => (p === null ? 0 : (p + 1) % data.length));
                        break;
                    default:
                }
            }}
            onFocus={() => {
                setDataIndex((p) => (p === null ? 0 : p));
            }}
            role="button"
            aria-label={`Showing ${title}`}
            tabIndex={0}
            width="100%"
            height="100%"
            display="flex"
            justifyContent="center"
            alignItems="center"
        >
            <Stack direction="column" width="100%" sx={{ maxWidth: '100%', overflow: 'hidden' }}>
                <Typography
                    sx={{
                        color: 'rgb(117, 117, 117)',
                        fontWeight: 500,
                        fontSize: '0.9rem',
                        pt: 1,
                    }}
                >
                    {dataIndex === null ? 'Latest' : labels[dataIndex] || 'Time'}
                </Typography>
                <Box
                    sx={{
                        display: 'grid',
                        gridTemplateColumns: '120px 1fr',
                        gap: 0.5,
                        alignItems: 'flex-end',
                        borderBottom: `solid 2px ${color}40`,
                        width: '100%'
                    }}
                >
                    <Typography sx={{ fontSize: '1.25rem', fontWeight: 500 }}>
                        {data[dataIndex ?? data.length - 1]?.toFixed(2)}{unit}
                    </Typography>

                    <Box sx={{ minWidth: 0 }}>
                        <SparkLineChart
                            height={60}
                            area
                            showHighlight
                            color={color}
                            onHighlightedAxisChange={(axisItems) => {
                                setDataIndex(axisItems[0]?.dataIndex ?? null);
                            }}
                            highlightedAxis={
                                dataIndex === null
                                    ? []
                                    : [{ axisId: 'time-axis', dataIndex: dataIndex }]
                            }
                            {...settings}
                        />
                    </Box>
                </Box>
            </Stack>
        </Box>
    );
}