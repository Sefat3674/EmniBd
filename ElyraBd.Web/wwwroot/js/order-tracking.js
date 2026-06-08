(function () {
    const container = document.getElementById('orderTracking');
    if (!container) return;

    const orderId = container.dataset.orderId;
    const badge = document.getElementById('orderStatusBadge');
    const timeline = document.getElementById('orderTimeline');

    function refreshStatus() {
        fetch(`/Orders/Status/${orderId}`)
            .then(r => r.ok ? r.json() : null)
            .then(data => {
                if (!data) return;
                if (badge) badge.textContent = data.statusLabel;
                if (timeline && data.timeline) {
                    timeline.innerHTML = data.timeline.map(step => `
                        <div class="timeline-step ${step.isCompleted ? 'completed' : ''} ${step.isCurrent ? 'current' : ''}">
                            <div class="timeline-dot"></div>
                            <div class="timeline-content">
                                <strong>${step.statusLabel}</strong>
                                ${step.changedAt ? `<small class="text-muted d-block">${new Date(step.changedAt).toLocaleString()}</small>` : ''}
                                ${step.note ? `<small class="text-muted">${step.note}</small>` : ''}
                            </div>
                        </div>`).join('');
                }
            })
            .catch(() => { });
    }

    setInterval(refreshStatus, 15000);
})();
