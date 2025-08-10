// Smooth scrolling for navigation links
document.addEventListener('DOMContentLoaded', function() {
    // Smooth scrolling
    const links = document.querySelectorAll('a[href^="#"]');
    links.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            const targetSection = document.querySelector(targetId);
            
            if (targetSection) {
                const headerHeight = document.querySelector('.header').offsetHeight;
                const targetPosition = targetSection.offsetTop - headerHeight;
                
                window.scrollTo({
                    top: targetPosition,
                    behavior: 'smooth'
                });
            }
        });
    });

    // Language toggle functionality
    window.toggleLanguage = function() {
        const langBtn = document.querySelector('.lang-btn');
        const currentLang = langBtn.textContent;
        
        if (currentLang === 'EN') {
            // Switch to English
            langBtn.textContent = 'RU';
            translateToEnglish();
        } else {
            // Switch to Russian
            langBtn.textContent = 'EN';
            translateToRussian();
        }
    };

    // Add scroll effect to header
    window.addEventListener('scroll', function() {
        const header = document.querySelector('.header');
        if (window.scrollY > 100) {
            header.style.background = 'rgba(255, 255, 255, 0.98)';
        } else {
            header.style.background = 'rgba(255, 255, 255, 0.95)';
        }
    });

    // Add animation to feature cards on scroll
    const observerOptions = {
        threshold: 0.1,
        rootMargin: '0px 0px -50px 0px'
    };

    const observer = new IntersectionObserver(function(entries) {
        entries.forEach(entry => {
            if (entry.isIntersecting) {
                entry.target.style.opacity = '1';
                entry.target.style.transform = 'translateY(0)';
            }
        });
    }, observerOptions);

    // Observe feature cards
    const featureCards = document.querySelectorAll('.feature-card');
    featureCards.forEach(card => {
        card.style.opacity = '0';
        card.style.transform = 'translateY(20px)';
        card.style.transition = 'opacity 0.6s ease, transform 0.6s ease';
        observer.observe(card);
    });

    // Download button click tracking
    const downloadButtons = document.querySelectorAll('.btn-primary');
    downloadButtons.forEach(button => {
        button.addEventListener('click', function() {
            // You can add analytics tracking here
            console.log('Download button clicked');
        });
    });
});

// Translation functions
function translateToEnglish() {
    const translations = {
        'hero-title': 'Automatic Windows Theme Switching',
        'hero-subtitle': 'Lightweight application for automatic Windows theme switching (Light/Dark) by schedule or manually from system tray',
        'btn-primary': 'Download Free',
        'btn-secondary': 'Open on GitHub',
        'features': 'Features',
        'download': 'Download',
        'screenshots': 'Screenshots',
        'docs': 'Documentation',
        'hero-badges-1': 'Windows 10/11',
        'hero-badges-2': 'Free',
        'hero-badges-3': 'Open Source',
        'hero-badges-4': 'MIT License'
    };

    Object.keys(translations).forEach(key => {
        const element = document.querySelector(`[data-translate="${key}"]`);
        if (element) {
            element.textContent = translations[key];
        }
    });
}

function translateToRussian() {
    const translations = {
        'hero-title': 'Автоматическое переключение тем Windows',
        'hero-subtitle': 'Легкое приложение для автоматического переключения тем Windows (Светлая/Тёмная) по расписанию или вручную из системного трея',
        'btn-primary': 'Скачать бесплатно',
        'btn-secondary': 'Открыть на GitHub',
        'features': 'Возможности',
        'download': 'Скачать',
        'screenshots': 'Скриншоты',
        'docs': 'Документация',
        'hero-badges-1': 'Windows 10/11',
        'hero-badges-2': 'Бесплатно',
        'hero-badges-3': 'Open Source',
        'hero-badges-4': 'MIT License'
    };

    Object.keys(translations).forEach(key => {
        const element = document.querySelector(`[data-translate="${key}"]`);
        if (element) {
            element.textContent = translations[key];
        }
    });
}

// Theme detection and application
function detectAndApplyTheme() {
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    
    if (prefersDark) {
        document.body.classList.add('dark-theme');
    } else {
        document.body.classList.remove('dark-theme');
    }
}

// Listen for theme changes
window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', detectAndApplyTheme);

// Initialize theme
detectAndApplyTheme();

// Add loading animation
window.addEventListener('load', function() {
    document.body.classList.add('loaded');
});

// Add some interactive effects
document.addEventListener('DOMContentLoaded', function() {
    // Add hover effects to buttons
    const buttons = document.querySelectorAll('.btn');
    buttons.forEach(button => {
        button.addEventListener('mouseenter', function() {
            this.style.transform = 'translateY(-2px) scale(1.02)';
        });
        
        button.addEventListener('mouseleave', function() {
            this.style.transform = 'translateY(0) scale(1)';
        });
    });

    // Add typing effect to hero title (optional)
    const heroTitle = document.querySelector('.hero-title');
    if (heroTitle) {
        const text = heroTitle.textContent;
        heroTitle.textContent = '';
        
        let i = 0;
        const typeWriter = () => {
            if (i < text.length) {
                heroTitle.textContent += text.charAt(i);
                i++;
                setTimeout(typeWriter, 50);
            }
        };
        
        // Start typing effect after a short delay
        setTimeout(typeWriter, 500);
    }
});

// Add mobile menu functionality
function createMobileMenu() {
    const nav = document.querySelector('.nav-links');
    const navContainer = document.querySelector('.nav-container');
    
    // Create mobile menu button
    const mobileMenuBtn = document.createElement('button');
    mobileMenuBtn.className = 'mobile-menu-btn';
    mobileMenuBtn.innerHTML = '☰';
    mobileMenuBtn.style.display = 'none';
    
    // Add mobile menu styles
    const style = document.createElement('style');
    style.textContent = `
        @media (max-width: 768px) {
            .mobile-menu-btn {
                display: block !important;
                background: none;
                border: none;
                font-size: 1.5rem;
                cursor: pointer;
                color: #333;
            }
            
            .nav-links {
                display: none;
                position: absolute;
                top: 100%;
                left: 0;
                right: 0;
                background: white;
                flex-direction: column;
                padding: 1rem;
                box-shadow: 0 2px 10px rgba(0,0,0,0.1);
            }
            
            .nav-links.active {
                display: flex;
            }
            
            .nav-container {
                position: relative;
            }
        }
    `;
    document.head.appendChild(style);
    
    // Insert mobile menu button
    navContainer.insertBefore(mobileMenuBtn, nav);
    
    // Toggle mobile menu
    mobileMenuBtn.addEventListener('click', function() {
        nav.classList.toggle('active');
    });
    
    // Close mobile menu when clicking on a link
    nav.addEventListener('click', function(e) {
        if (e.target.tagName === 'A') {
            nav.classList.remove('active');
        }
    });
}

// Initialize mobile menu
createMobileMenu();
