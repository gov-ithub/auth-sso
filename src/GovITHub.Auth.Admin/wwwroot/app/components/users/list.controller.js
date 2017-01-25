(function () {
    'use strict';

    angular
        .module('authAdminPanel')
        .controller('UsersListController', UsersListController);

    UsersListController.$inject = ['User', '$rootScope', '$log', '$scope', 'UserIdentityService']

    function UsersListController(User, $rootScope, $log, $scope, UserIdentityService) {

        var vm = this;
        vm.pagination = {
            currentPage: 1,
            itemsPerPage: 10,
            totalItems: 150,
            maxDisplayedPages: 5
        };
        vm.sortBy = 'Name';
        vm.sortAscending = true;

        vm.search = function () {
            User.filter({
                q: vm.query,
                currentPage: vm.pagination.currentPage,
                itemsPerPage: vm.pagination.itemsPerPage,
                sortBy: vm.sortBy,
                sortAscending: vm.sortAscending,
                organizationId: $scope.currentUser.organizationId
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
            $rootScope.goto('index.users_edit', { id: id });
        };

        vm.delete = function (id) {
            User.delete({ id: id, organizationId: $scope.currentUser.organizationId }).$promise
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