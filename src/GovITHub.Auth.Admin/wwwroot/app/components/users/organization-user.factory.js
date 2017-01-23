(function () {
    'use strict';

    angular
        .module('authAdminPanel')
        .factory("User", User);

    User.$inject = ['$resource'];

    function User($resource) {
        return $resource('/api/users/:id', { id: '@id' }, {
            filter: { method: 'GET' },
            update: { method: 'PUT' }
        });
    };
})();