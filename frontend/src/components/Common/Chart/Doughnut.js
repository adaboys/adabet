import React from 'react';
import { Chart as ChartJS, ArcElement, Tooltip, Legend } from 'chart.js';
import { Doughnut as DoughnutChartJS } from 'react-chartjs-2';

ChartJS.register(ArcElement, Tooltip, Legend);

const Doughnut = ({ data }) => {
    const options = {
        responsive: true,
        plugins: {
            legend: false,
            tooltip: {
                callbacks: {
                    label: ({ raw }) => ` ${raw}%`
                }
            }
        },
    }

    return (
        <DoughnutChartJS data={data} options={options} />
    );
};

export default Doughnut;