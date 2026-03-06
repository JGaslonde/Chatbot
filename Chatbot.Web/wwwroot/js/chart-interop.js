// Chart.js Interop Service
// Provides JavaScript interop for rendering Chart.js charts in Blazor components

const charts = new Map();

export function initializeChart(canvasId, chartConfig) {
    try {
        // Get the canvas element
        const canvas = document.getElementById(canvasId);
        if (!canvas) {
            console.error(`Canvas element with id '${canvasId}' not found.`);
            return false;
        }

        // Destroy existing chart if it exists
        if (charts.has(canvasId)) {
            charts.get(canvasId).destroy();
        }

        // Create new chart
        const ctx = canvas.getContext('2d');
        const chartInstance = new Chart(ctx, chartConfig);

        // Store chart reference
        charts.set(canvasId, chartInstance);
        return true;
    } catch (error) {
        console.error(`Error initializing chart ${canvasId}:`, error);
        return false;
    }
}

export function updateChart(canvasId, newData) {
    try {
        const chart = charts.get(canvasId);
        if (!chart) {
            console.warn(`Chart with id '${canvasId}' not found.`);
            return false;
        }

        // Update chart data
        if (newData.labels) {
            chart.data.labels = newData.labels;
        }
        if (newData.datasets) {
            chart.data.datasets = newData.datasets;
        }

        // Refresh chart
        chart.update();
        return true;
    } catch (error) {
        console.error(`Error updating chart ${canvasId}:`, error);
        return false;
    }
}

export function destroyChart(canvasId) {
    try {
        const chart = charts.get(canvasId);
        if (chart) {
            chart.destroy();
            charts.delete(canvasId);
            return true;
        }
        return false;
    } catch (error) {
        console.error(`Error destroying chart ${canvasId}:`, error);
        return false;
    }
}

export function resizeChart(canvasId) {
    try {
        const chart = charts.get(canvasId);
        if (chart) {
            chart.resize();
            return true;
        }
        return false;
    } catch (error) {
        console.error(`Error resizing chart ${canvasId}:`, error);
        return false;
    }
}
