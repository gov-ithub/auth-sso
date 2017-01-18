(function () {
    'use strict';

    angular
        .module('authAdminPanel')
        .factory("OrganizationUser", OrganizationUser);

    OrganizationUser.$inject = ['$q', '$resource', '$log'];

    function OrganizationUser($q, $resource, $log) {
        return $resource('/api/users/:id', { id: '@id' }, {
            filter: { method: 'GET' },
            update: { method: 'PUT' }
        });
    };
})();