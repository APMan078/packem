import React from 'react';
import { Line } from 'react-chartjs-2';

import {
  Chart,
  ArcElement,
  LineElement,
  BarElement,
  PointElement,
  BarController,
  BubbleController,
  DoughnutController,
  LineController,
  PieController,
  PolarAreaController,
  RadarController,
  ScatterController,
  CategoryScale,
  LinearScale,
  LogarithmicScale,
  RadialLinearScale,
  TimeScale,
  TimeSeriesScale,
  Decimation,
  Filler,
  Legend,
  Title,
  Tooltip,
} from 'chart.js';

Chart.register(
  ArcElement,
  LineElement,
  BarElement,
  PointElement,
  BarController,
  BubbleController,
  DoughnutController,
  LineController,
  PieController,
  PolarAreaController,
  RadarController,
  ScatterController,
  CategoryScale,
  LinearScale,
  LogarithmicScale,
  RadialLinearScale,
  TimeScale,
  TimeSeriesScale,
  Decimation,
  Filler,
  Legend,
  Title,
  Tooltip,
);
function LineChart({ data }): any {
  console.log('Data', data);
  function colorItems(tolltipItem) {
    const tooltipBackgroundColor =
      tolltipItem.tooltip.labelColors[0].borderColor;
    return tooltipBackgroundColor;
  }
  return (
    Object.keys(data).length > 0 && (
      <Line
        data={{
          labels: [...data.labels],
          datasets: [
            {
              label: data.dataSets[0].label,
              data: [...data.dataSets[0].data],
              borderColor: 'rgba(112, 112, 112, 1)',
              backgroundColor: 'rgba(112, 112, 112, 1)',
              borderWidth: 3,
              pointRadius: 5,
            },
            {
              label: data.dataSets[1].label,
              data: [...data.dataSets[1].data],
              borderColor: 'rgba(29, 204, 14, 1)',
              backgroundColor: 'rgba(29, 204, 14, 1)',
              borderWidth: 3,
              pointRadius: 5,
            },
          ],
        }}
        height={400}
        width={600}
        options={{
          interaction: {
            mode: 'point',
          },
          maintainAspectRatio: false,
          scales: {
            y: {
              beginAtZero: true,
              grid: {
                display: false,
              },
            },
            x: {
              grid: {
                display: false,
              },
            },
          },
          plugins: {
            tooltip: {
              mode: 'nearest',
              intersect: false,
              xAlign: 'center',
              cornerRadius: 20,
              padding: 10,
              displayColors: false,
              bodyFont: {
                size: 20,
              },
              backgroundColor: colorItems,
              callbacks: {
                label(context) {
                  return context.formattedValue;
                },
                title(context) {
                  return '';
                },
              },
            },
            legend: {
              position: 'top',
              align: 'start',
              labels: {
                padding: 50,
                usePointStyle: true,
                pointStyle: 'circle',
                font: {
                  size: 14,
                  weight: '600',
                },
              },
            },
          },
        }}
      />
    )
  );
}

export default React.memo(LineChart);
