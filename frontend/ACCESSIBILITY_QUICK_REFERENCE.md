# Accessibility Quick Reference Card

## 🚀 Quick Start Testing

### Test Keyboard Navigation (30 seconds)
1. Press `Tab` - Skip link should appear
2. Press `Enter` - Should jump to main content
3. Keep pressing `Tab` - All interactive elements should be reachable
4. Look for blue focus outline on each element

### Test Screen Reader (2 minutes)
**Windows**: Press `Ctrl+Alt+N` to start NVDA
**Mac**: Press `Cmd+F5` to start VoiceOver

Navigate with arrow keys and listen for proper announcements.

---

## ✅ Code Patterns to Use

### Buttons with Icons
```vue
<!-- ✅ CORRECT -->
<button @click="handleDelete" aria-label="Delete item">
  <TrashIcon aria-hidden="true" />
</button>

<!-- ❌ WRONG -->
<button @click="handleDelete">
  <TrashIcon />
</button>
```

### Links
```vue
<!-- ✅ CORRECT -->
<RouterLink to="/billing" aria-label="View billing details">
  <CreditCardIcon aria-hidden="true" />
  <span>Billing</span>
</RouterLink>

<!-- ❌ WRONG -->
<RouterLink to="/billing">
  <CreditCardIcon />
</RouterLink>
```

### Form Inputs
```vue
<!-- ✅ CORRECT -->
<label for="email">Email Address</label>
<input 
  id="email" 
  type="email" 
  v-model="email"
  aria-describedby="email-error"
/>
<p id="email-error" v-if="error">{{ error }}</p>

<!-- ❌ WRONG -->
<input type="email" placeholder="Email" v-model="email" />
```

### Progress Bars
```vue
<!-- ✅ CORRECT -->
<div 
  role="progressbar" 
  aria-valuenow="52" 
  aria-valuemin="0" 
  aria-valuemax="100"
  aria-label="Data usage: 52% of 10GB used"
>
  <div class="bar" style="width: 52%"></div>
</div>

<!-- ❌ WRONG -->
<div class="progress">
  <div style="width: 52%"></div>
</div>
```

### Navigation
```vue
<!-- ✅ CORRECT -->
<nav aria-label="Main navigation">
  <RouterLink to="/" aria-current="page">Home</RouterLink>
  <RouterLink to="/about">About</RouterLink>
</nav>

<!-- ❌ WRONG -->
<div class="nav">
  <a href="/">Home</a>
  <a href="/about">About</a>
</div>
```

### Images
```vue
<!-- ✅ CORRECT - Informative -->
<img src="chart.png" alt="Monthly data usage chart" />

<!-- ✅ CORRECT - Decorative -->
<img src="decoration.png" alt="" aria-hidden="true" />
<UserIcon aria-hidden="true" />

<!-- ❌ WRONG -->
<img src="chart.png" />
```

### Dynamic Content
```vue
<!-- ✅ CORRECT -->
<div role="status" aria-live="polite">
  {{ statusMessage }}
</div>

<!-- For urgent alerts -->
<div role="alert" aria-live="assertive">
  {{ errorMessage }}
</div>

<!-- ❌ WRONG -->
<div>{{ statusMessage }}</div>
```

---

## 🎨 CSS Classes Available

```vue
<!-- Skip link (already in MainLayout) -->
<a href="#main" class="skip-link">Skip to content</a>

<!-- Screen reader only text -->
<span class="sr-only">Loading...</span>

<!-- Focus visible ring -->
<button class="focus-visible-ring">Click me</button>
```

---

## 🧪 Test Commands

```bash
# Run accessibility tests
npm run test:accessibility

# Run all tests
npm run test:all

# View test report
npm run test:report
```

---

## 📋 Pre-Commit Checklist

Before committing new UI components:

- [ ] All buttons/links have descriptive labels
- [ ] Icons are marked `aria-hidden="true"`
- [ ] Form inputs have associated `<label>` elements
- [ ] Focus is visible on all interactive elements
- [ ] Keyboard navigation works (test with Tab)
- [ ] Color contrast is sufficient (use browser DevTools)
- [ ] Dynamic content uses `aria-live` regions
- [ ] Semantic HTML used (`<nav>`, `<main>`, `<button>`)

---

## 🔍 Browser DevTools

### Chrome/Edge - Check Accessibility
1. Open DevTools (`F12`)
2. Go to **Lighthouse** tab
3. Select **Accessibility** only
4. Click **Generate report**
5. Fix any issues with score < 90

### Firefox - Check Contrast
1. Open DevTools (`F12`)
2. Go to **Accessibility** tab
3. Enable **Check for issues**
4. Review contrast warnings

---

## 🎯 Common Mistakes

| ❌ Don't | ✅ Do |
|---------|------|
| `<div onclick="...">` | `<button @click="...">` |
| `<a href="#">Action</a>` | `<button>Action</button>` |
| Icon without label | Icon with `aria-label` |
| `placeholder` as label | Proper `<label>` element |
| Decorative icon without `aria-hidden` | `aria-hidden="true"` |
| Missing focus styles | Visible focus indicator |

---

## 📞 Need Help?

- Check `ACCESSIBILITY_GUIDE.md` for detailed documentation
- Run `npm run test:accessibility` to find issues
- Use browser DevTools Lighthouse for audits

---

**Remember**: Accessibility is not optional - it's a requirement!
