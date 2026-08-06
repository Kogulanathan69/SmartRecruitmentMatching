(() => {
  'use strict';
  const apiBase = window.NEXHIRE_API_BASE || '';

  function token() { return localStorage.getItem('accessToken') || ''; }
  function escapeHtml(value) {
    return String(value ?? '').replace(/[&<>'"]/g, character => ({
      '&': '&amp;', '<': '&lt;', '>': '&gt;', "'": '&#39;', '"': '&quot;'
    })[character]);
  }
  async function api(path, options = {}) {
    const headers = new Headers(options.headers || {});
    headers.set('Accept', 'application/json');
    if (options.body && !(options.body instanceof FormData)) headers.set('Content-Type', 'application/json');
    if (token()) headers.set('Authorization', `Bearer ${token()}`);
    const response = await fetch(`${apiBase}${path}`, { ...options, headers });
    const text = await response.text();
    const data = text ? JSON.parse(text) : null;
    if (!response.ok) {
      const error = new Error(data?.detail || data?.title || `Request failed (${response.status})`);
      error.status = response.status; error.code = data?.code || data?.extensions?.code; error.payload = data;
      throw error;
    }
    return data;
  }
  function setBusy(button, busy, label = 'Please wait…') {
    if (!button) return;
    if (busy) { button.dataset.originalText = button.textContent; button.textContent = label; button.disabled = true; }
    else { button.textContent = button.dataset.originalText || button.textContent; button.disabled = false; }
  }
  function showMessage(element, message, type = 'error') {
    if (!element) return;
    element.className = `member5-message show ${type}`;
    element.textContent = message;
  }
  function clearMessage(element) { if (element) { element.className = 'member5-message'; element.textContent = ''; } }
  function formatDate(value) { return value ? new Intl.DateTimeFormat(undefined, { dateStyle: 'medium', timeStyle: 'short' }).format(new Date(value)) : '—'; }
  function money(amount, currency) { try { return new Intl.NumberFormat(undefined, { style: 'currency', currency }).format(amount); } catch { return `${currency} ${amount}`; } }
  window.Member5 = { api, escapeHtml, setBusy, showMessage, clearMessage, formatDate, money };
})();
