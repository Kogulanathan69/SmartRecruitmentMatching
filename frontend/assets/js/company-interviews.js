(() => {
  'use strict';
  const { api, escapeHtml, setBusy, showMessage, clearMessage, formatDate } = window.Member5;
  const list = document.getElementById('interview-list');
  const message = document.getElementById('message');
  const form = document.getElementById('schedule-form');

  async function load() {
    list.className = 'member5-loading'; list.textContent = 'Loading interviews…';
    try {
      const data = await api('/api/Interviews/company?page=1&pageSize=100');
      const items = data.items || [];
      if (!items.length) { list.className = 'member5-empty'; list.textContent = 'No interviews scheduled.'; return; }
      list.className = 'member5-grid';
      list.innerHTML = items.map(item => `<article class="member5-card"><span class="member5-status">${escapeHtml(item.status)}</span><h3>${escapeHtml(item.candidateName || item.candidateProfileId)}</h3><p>${escapeHtml(item.jobTitle || 'Application')}</p><p class="member5-muted">${formatDate(item.scheduledAtUtc)} · ${escapeHtml(item.mode)} · ${item.durationMinutes} min</p><p>${escapeHtml(item.meetingLink || item.location || item.contactPhone || 'Details pending')}</p><div class="member5-actions">${['Scheduled','Rescheduled',1,2].includes(item.status) ? `<button class="member5-button secondary" data-action="complete" data-id="${item.interviewId}">Complete</button><button class="member5-button danger" data-action="cancel" data-id="${item.interviewId}">Cancel</button>` : ''}${['Completed',3].includes(item.status) ? `<button class="member5-button" data-action="score" data-id="${item.interviewId}">Add score</button>` : ''}</div></article>`).join('');
    } catch (error) { list.className = 'member5-empty'; list.textContent = 'Unable to load interviews.'; showMessage(message, error.message); }
  }

  form.addEventListener('submit', async event => {
    event.preventDefault(); clearMessage(message); const button = form.querySelector('button[type="submit"]'); setBusy(button, true);
    const values = Object.fromEntries(new FormData(form));
    const payload = { applicationId: values.applicationId, scheduledAtUtc: new Date(values.scheduledAtUtc).toISOString(), durationMinutes: Number(values.durationMinutes), mode: Number(values.mode), meetingLink: values.meetingLink || null, location: values.location || null, contactPhone: values.contactPhone || null, notes: values.notes || null };
    try { await api('/api/Interviews', { method: 'POST', body: JSON.stringify(payload) }); form.reset(); showMessage(message, 'Interview scheduled.', 'success'); await load(); }
    catch (error) { showMessage(message, error.message); } finally { setBusy(button, false); }
  });

  list.addEventListener('click', async event => {
    const button = event.target.closest('button[data-action]'); if (!button) return;
    const id = button.dataset.id, action = button.dataset.action; clearMessage(message); setBusy(button, true);
    try {
      if (action === 'cancel') { const reason = prompt('Cancellation reason:'); if (!reason) return; await api(`/api/Interviews/${id}/cancel`, { method: 'POST', body: JSON.stringify({ reason }) }); }
      if (action === 'complete') await api(`/api/Interviews/${id}/complete`, { method: 'POST' });
      if (action === 'score') { const score = Number(prompt('Score (0-100):')); const feedback = prompt('Feedback:'); if (!Number.isFinite(score) || !feedback) return; await api(`/api/Interviews/${id}/score`, { method: 'POST', body: JSON.stringify({ score, feedback }) }); }
      showMessage(message, 'Interview updated.', 'success'); await load();
    } catch (error) { showMessage(message, error.message); } finally { setBusy(button, false); }
  });
  load();
})();
