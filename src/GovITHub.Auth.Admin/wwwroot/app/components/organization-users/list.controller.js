(function () {
    'use strict';
    angular
        .module('authAdminPanel')
        .controller('OrganizationUsersListController', OrganizationUsersListController);

    OrganizationUsersListController.$inject = ['OrganizationUser', '$rootScope', '$log', '$scope', 'UserIdentityService']

    function OrganizationUsersListController(OrganizationUser, $rootScope, $log, $scope, UserIdentityService) {

        var vm = this;
        vm.pagination = {
            currentPage: 1,
            itemsPerPage: 10,
            totalItems: 150,
            maxDisplayedPages: 5
        };
        vm.sortBy = 'OrganizationId';
        vm.sortAscending = true;

        vm.search = function () {
            OrganizationUser.filter({
                q: vm.query,
                currentPage: vm.pagination.currentPage,
                itemsPerPage: vm.pagination.itemsPerPage,
                sortBy: vm.sortBy,
                sortAscending: vm.sortAscending
            }).$promise
                .then(function (result) {
                    vm.items = result.list;
                    vm.pagination.totalItems = result.totalItems
                }).catch(function (err) {
                    $log.error(err);
                    vm.error = err;
                });
        };

        vm.sort = function (sortBy) {
            vm.sortBy = sortBy;
            vm.sortAscending = !vm.sortAscending;
            vm.search();
        };

        vm.gotoEdit = function (id) {
            $rootScope.goto('index.organization-users_edit', { id: id });
        };

        vm.delete = function (id) {
            OrganizationUser.delete({ id: id }).$promise
                .then(function (response) {
                    vm.search();
                }).catch(function (err) {
                    $log.error(err);
                    vm.error = err;
                });
        };

        vm.search();
    };
})();