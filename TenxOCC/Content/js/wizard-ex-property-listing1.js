'use strict';

(function () {
    // Init custom option check
    if (window.Helpers) {
        window.Helpers.initCustomOptionCheck();
    }

    const flatpickrRange = document.querySelector('.flatpickr'),
        phoneMask = document.querySelector('.contact-number-mask'),
        plCountry = $('#plCountry'),
        plFurnishingDetailsSuggestionEl = document.querySelector('#plFurnishingDetails');

    // Phone Number Input Mask
    if (phoneMask && window.Cleave) {
        new Cleave(phoneMask, {
            phone: true,
            phoneRegionCode: 'US'
        });
    }

    // Select2 (Country)
    if (plCountry && plCountry.length) {
        plCountry.wrap('<div class="position-relative"></div>');
        plCountry.select2({
            placeholder: 'Select country',
            dropdownParent: plCountry.parent()
        });
    }

    // Flatpickr
    if (flatpickrRange && window.flatpickr) {
        flatpickrRange.flatpickr();
    }

    // Tagify
    if (plFurnishingDetailsSuggestionEl && window.Tagify) {
        new Tagify(plFurnishingDetailsSuggestionEl, {
            whitelist: [
                'Fridge', 'TV', 'AC', 'WiFi', 'RO', 'Washing Machine',
                'Sofa', 'Bed', 'Dining Table', 'Microwave', 'Cupboard'
            ],
            maxTags: 10
        });
    }

    // Wizard
    const wizardPropertyListing = document.querySelector('#wizard-property-listing');
    if (!wizardPropertyListing) return;

    const wizardForm = wizardPropertyListing.querySelector('#wizard-property-listing-form');
    if (!wizardForm) return;

    const wizardNext = wizardForm.querySelectorAll('.btn-next');
    const wizardPrev = wizardForm.querySelectorAll('.btn-prev');

    const stepper = new Stepper(wizardPropertyListing, {
        linear: false,  
        animation: true
    });

    // Next buttons
    wizardNext.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            stepper.next();
        });
    });

    // Previous buttons
    wizardPrev.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            stepper.previous();
        });
    });

})();
