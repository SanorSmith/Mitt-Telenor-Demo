# Accessibility Compliance Guide

## Overview
This project now implements **WCAG 2.1 Level AA** accessibility standards to ensure the application is usable by everyone, including people with disabilities.

---

## ✅ Implemented Accessibility Features

### 1. **Skip Navigation Link**
- **What it does**: Allows keyboard users to skip directly to main content
- **How to use**: Press `Tab` when the page loads - a "Skip to main content" link will appear
- **Location**: Top of every page in `MainLayout.vue`

### 2. **Keyboard Navigation**
- **Full keyboard support**: Navigate using `Tab`, `Shift+Tab`, `Enter`, and `Space`
- **Visible focus indicators**: All interactive elements show a blue outline when focused
- **Focus management**: Logical tab order throughout the application

### 3. **ARIA Labels & Landmarks**
- **Screen reader support**: All interactive elements have descriptive labels
- **Semantic HTML**: Proper use of `<nav>`, `<main>`, `<header>`, `<footer>`
- **ARIA attributes**: 
  - `aria-label` for buttons and links
  - `aria-current` for active navigation items
  - `aria-hidden` for decorative icons
  - `role="progressbar"` for usage indicators

### 4. **Progress Bars with Screen Reader Support**
- **Usage statistics**: Data, voice, and SMS usage have proper ARIA attributes
- **Attributes used**:
  - `role="progressbar"`
  - `aria-valuenow`, `aria-valuemin`, `aria-valuemax`
  - Descriptive `aria-label` (e.g., "Data usage: 52% of 10 GB used")

### 5. **Enhanced Focus Visibility**
- **Global focus styles**: 2px solid blue outline with 2px offset
- **Custom focus classes**: `.focus-visible-ring` utility class
- **High contrast**: Meets WCAG contrast requirements

### 6. **Mobile Navigation Accessibility**
- **Touch targets**: Minimum 44x44px (WCAG requirement)
- **ARIA labels**: Each navigation item properly labeled
- **Current page indicator**: `aria-current="page"` for active items

---

## 🎯 How to Use Accessibility Features

### For Keyboard Users

#### **Basic Navigation**
1. Press `Tab` to move forward through interactive elements
2. Press `Shift+Tab` to move backward
3. Press `Enter` or `Space` to activate buttons/links
4. Press `Esc` to close modals (when implemented)

#### **Skip to Main Content**
1. When page loads, press `Tab` once
2. You'll see "Skip to main content" link appear
3. Press `Enter` to jump directly to main content
4. Bypasses header navigation

#### **Navigate the Dashboard**
1. Use `Tab` to move through:
   - Header navigation
   - Quick action cards
   - Usage statistics
   - Recent activity
2. Press `Enter` on any card to navigate to that section

### For Screen Reader Users

#### **Recommended Screen Readers**
- **Windows**: NVDA (free) or JAWS
- **macOS**: VoiceOver (built-in)
- **Linux**: Orca
- **Mobile**: TalkBack (Android) or VoiceOver (iOS)

#### **Navigation Landmarks**
The application uses proper landmarks for easy navigation:
- `<header>` - Site header with logo and navigation
- `<main>` - Main content area (labeled "Main content")
- `<nav>` - Navigation menus (labeled appropriately)
- `<footer>` - Site footer with links

#### **Usage Statistics**
Progress bars announce:
- Current usage amount
- Total available
- Percentage used
- Example: "Data usage: 52% of 10 GB used"

### For Users with Low Vision

#### **High Contrast Mode**
- Focus indicators are clearly visible
- 2px solid blue outline (#0066CC)
- Works with Windows High Contrast mode

#### **Text Scaling**
- Application supports browser zoom up to 200%
- Layout remains functional at all zoom levels
- No horizontal scrolling required

---

## 🧪 Testing Accessibility

### Run Automated Tests

```bash
# Run all accessibility tests
npm run test:accessibility

# Run Lighthouse accessibility audit
npm run build
npm run preview
# Then run Lighthouse in Chrome DevTools
```

### Manual Testing Checklist

#### **Keyboard Navigation**
- [ ] Can navigate entire app using only keyboard
- [ ] All interactive elements are reachable
- [ ] Focus order is logical
- [ ] No keyboard traps
- [ ] Skip link appears and works

#### **Screen Reader Testing**
- [ ] All images have alt text or aria-hidden
- [ ] Form inputs have associated labels
- [ ] Buttons have descriptive text
- [ ] Links describe their destination
- [ ] Dynamic content changes are announced

#### **Visual Testing**
- [ ] Focus indicators are clearly visible
- [ ] Text has sufficient contrast (4.5:1 minimum)
- [ ] UI works at 200% zoom
- [ ] No content is lost when zoomed

---

## 📋 Accessibility Checklist for New Features

When adding new features, ensure:

### **Interactive Elements**
```vue
<!-- ✅ Good: Button with aria-label -->
<button @click="handleAction" aria-label="Delete item">
  <TrashIcon aria-hidden="true" />
</button>

<!-- ❌ Bad: Button without label -->
<button @click="handleAction">
  <TrashIcon />
</button>
```

### **Images**
```vue
<!-- ✅ Good: Decorative icon -->
<UserIcon aria-hidden="true" />

<!-- ✅ Good: Informative image -->
<img src="chart.png" alt="Monthly usage chart showing 5GB used" />

<!-- ❌ Bad: Missing alt text -->
<img src="chart.png" />
```

### **Forms**
```vue
<!-- ✅ Good: Label associated with input -->
<label for="email">Email Address</label>
<input id="email" type="email" v-model="email" />

<!-- ❌ Bad: No label -->
<input type="email" placeholder="Email" v-model="email" />
```

### **Navigation**
```vue
<!-- ✅ Good: Semantic nav with aria-label -->
<nav aria-label="Main navigation">
  <RouterLink to="/" aria-current="page">Home</RouterLink>
</nav>

<!-- ❌ Bad: Generic div -->
<div class="nav">
  <a href="/">Home</a>
</div>
```

### **Dynamic Content**
```vue
<!-- ✅ Good: Live region for updates -->
<div role="status" aria-live="polite">
  {{ statusMessage }}
</div>

<!-- ❌ Bad: No announcement -->
<div>{{ statusMessage }}</div>
```

---

## 🎨 CSS Classes for Accessibility

### **Screen Reader Only**
```css
.sr-only {
  /* Hides visually but keeps for screen readers */
  position: absolute;
  width: 1px;
  height: 1px;
  padding: 0;
  margin: -1px;
  overflow: hidden;
  clip: rect(0, 0, 0, 0);
  white-space: nowrap;
  border: 0;
}
```

Usage:
```vue
<span class="sr-only">Loading...</span>
```

### **Focus Visible Ring**
```css
.focus-visible-ring {
  @apply focus-visible:outline-none focus-visible:ring-2 
         focus-visible:ring-primary-500 focus-visible:ring-offset-2;
}
```

Usage:
```vue
<button class="btn focus-visible-ring">Click me</button>
```

### **Skip Link**
```css
.skip-link {
  /* Hidden until focused */
  position: absolute;
  left: 0;
  top: 0;
  transform: translateY(-100%);
}

.skip-link:focus {
  transform: translateY(0);
}
```

---

## 🔧 Configuration Files

### **Lighthouse Configuration**
File: `frontend/lighthouserc.json`

```json
{
  "ci": {
    "assert": {
      "assertions": {
        "categories:accessibility": ["error", {"minScore": 0.9}]
      }
    }
  }
}
```

This ensures accessibility score stays above 90%.

### **Playwright Accessibility Tests**
File: `frontend/tests/e2e/comprehensive-test-suite.spec.ts`

Tests include:
- Semantic HTML validation
- Keyboard navigation
- ARIA attributes
- Focus management
- Color contrast
- Screen reader compatibility

---

## 📚 Resources

### **WCAG Guidelines**
- [WCAG 2.1 Quick Reference](https://www.w3.org/WAI/WCAG21/quickref/)
- [WebAIM WCAG Checklist](https://webaim.org/standards/wcag/checklist)

### **Testing Tools**
- [axe DevTools](https://www.deque.com/axe/devtools/) - Browser extension
- [WAVE](https://wave.webaim.org/) - Web accessibility evaluation tool
- [Lighthouse](https://developers.google.com/web/tools/lighthouse) - Built into Chrome

### **Screen Readers**
- [NVDA](https://www.nvaccess.org/) - Free Windows screen reader
- [VoiceOver Guide](https://www.apple.com/accessibility/voiceover/) - macOS/iOS
- [TalkBack](https://support.google.com/accessibility/android/answer/6283677) - Android

---

## 🐛 Common Issues & Solutions

### **Issue: Focus not visible**
**Solution**: Add `.focus-visible-ring` class or ensure custom focus styles

### **Issue: Screen reader not announcing changes**
**Solution**: Use `aria-live` regions for dynamic content

### **Issue: Keyboard trap in modal**
**Solution**: Implement focus trap with proper escape handling

### **Issue: Images not described**
**Solution**: Add `alt` attribute or `aria-hidden="true"` for decorative images

---

## 📊 Compliance Status

| WCAG Criterion | Level | Status | Notes |
|----------------|-------|--------|-------|
| 1.1.1 Non-text Content | A | ✅ Pass | All images have alt text or aria-hidden |
| 1.3.1 Info and Relationships | A | ✅ Pass | Semantic HTML, proper headings |
| 1.4.3 Contrast (Minimum) | AA | ✅ Pass | 4.5:1 for text, 3:1 for UI |
| 2.1.1 Keyboard | A | ✅ Pass | Full keyboard navigation |
| 2.1.2 No Keyboard Trap | A | ✅ Pass | No traps detected |
| 2.4.1 Bypass Blocks | A | ✅ Pass | Skip navigation link |
| 2.4.3 Focus Order | A | ✅ Pass | Logical tab order |
| 2.4.7 Focus Visible | AA | ✅ Pass | Clear focus indicators |
| 3.2.3 Consistent Navigation | AA | ✅ Pass | Navigation consistent across pages |
| 4.1.2 Name, Role, Value | A | ✅ Pass | Proper ARIA labels |

**Overall Compliance**: WCAG 2.1 Level AA ✅

---

## 🚀 Quick Start

1. **Test keyboard navigation**: Press `Tab` and navigate the app
2. **Test skip link**: Press `Tab` on page load, then `Enter`
3. **Test screen reader**: Enable your OS screen reader and navigate
4. **Run automated tests**: `npm run test:accessibility`
5. **Check Lighthouse score**: Build and run Lighthouse audit

---

## 💡 Best Practices

1. **Always use semantic HTML** - `<button>` not `<div onclick>`
2. **Provide text alternatives** - Alt text, aria-labels, or sr-only text
3. **Maintain focus order** - Ensure logical tab sequence
4. **Test with real users** - Nothing beats actual user testing
5. **Keep it simple** - Clear, concise labels and instructions

---

## 📞 Support

For accessibility issues or questions:
- Create an issue in the project repository
- Tag with `accessibility` label
- Include screen reader/browser details if applicable

---

**Last Updated**: January 2026
**Maintained By**: Development Team
**Accessibility Standard**: WCAG 2.1 Level AA
