import { test, expect } from '@playwright/test'

test.describe('Authentication Flow', () => {
  test('should display login page', async ({ page }) => {
    await page.goto('/auth/login')
    await expect(page.locator('h2')).toContainText('Welcome Back')
    await expect(page.locator('input[type="email"]')).toBeVisible()
    await expect(page.locator('input[type="password"]')).toBeVisible()
  })

  test('should navigate to register page', async ({ page }) => {
    await page.goto('/auth/login')
    await page.click('text=Sign up')
    await expect(page).toHaveURL('/auth/register')
    await expect(page.locator('h2')).toContainText('Create Account')
  })

  test('should show validation errors for invalid email', async ({ page }) => {
    await page.goto('/auth/login')
    await page.fill('input[type="email"]', 'invalid-email')
    await page.fill('input[type="password"]', 'password')
    await page.click('button[type="submit"]')
    await expect(page.locator('text=valid email')).toBeVisible()
  })

  test('should register new user', async ({ page }) => {
    await page.goto('/auth/register')
    await page.fill('input[id="firstName"]', 'John')
    await page.fill('input[id="lastName"]', 'Doe')
    await page.fill('input[type="email"]', 'john.doe@example.com')
    await page.fill('input[type="password"]', 'Password123!')
    await page.click('button[type="submit"]')
    
    await page.waitForURL('/')
    await expect(page.locator('h1')).toContainText('Welcome back')
  })

  test('should login existing user', async ({ page }) => {
    await page.goto('/auth/login')
    await page.fill('input[type="email"]', 'test@example.com')
    await page.fill('input[type="password"]', 'Password123!')
    await page.click('button[type="submit"]')
    
    await page.waitForURL('/')
    await expect(page).toHaveURL('/')
  })

  test('should logout user', async ({ page }) => {
    await page.goto('/auth/login')
    await page.fill('input[type="email"]', 'test@example.com')
    await page.fill('input[type="password"]', 'Password123!')
    await page.click('button[type="submit"]')
    
    await page.waitForURL('/')
    await page.click('button[aria-label="Logout"]')
    
    await expect(page).toHaveURL('/auth/login')
  })
})
