(function () {
    'use strict';

    angular
      .module('authAdminPanel')
      .config(routerConfig);

    /** @ngInject */
    function routerConfig($stateProvider, $urlRouterProvider, $resourceProvider) {
        $resourceProvider.defaults.stripTrailingSlashes = false;

        $stateProvider
          .state('index', {
              abstract: true,
              url: "/index",
              templateUrl: "app/components/common/content.html"
          })
          .state('index.main', {
              url: "/main",
              templateUrl: "app/components/main/main.html",
              data: {
                  pageTitle: 'Dashboard'
              }
          })
            .state('index.samples', {
                url: "/samples",
                controller: "SamplesListController as vm",
                templateUrl: "app/components/samples/list.html",
                data: {
                    pageTitle: 'Samples'
                }
            })
            .state('index.samples_new', {
                url: "/samples/new",
                controller: "SamplesEditController as vm",
                templateUrl: "app/components/samples/edit.html",
                data: {
                    pageTitle: 'Samples'
                },
                resolve: {
                    status: [function () {
                        return { edit: false };
                    }],
                    id: [function () {
                        return null;
                    }]
                }
            })
            .state('index.samples_edit', {
                url: "/samples/:id",
                controller: "SamplesEditController as vm",
                templateUrl: "app/components/samples/edit.html",
                data: {
                    pageTitle: 'Samples'
                },
                resolve: {
                    status: [function () {
                        return { edit: true };
                    }],
                    id: ['$stateParams', function ($stateParams) {
                        return $stateParams.id;
                    }]
                }
            })
            .state('index.organizations', {
                url: "/organizations",
                controller: "OrganizationsListController as vm",
                templateUrl: "app/components/organizations/list.html",
                data: {
                    pageTitle: 'Organizations'
                }
            })
            .state('index.organization_new', {
                url: "/organizations/new",
                controller: "OrganizationsEditController as vm",
                templateUrl: "app/components/organizations/edit.html",
                data: {
                    pageTitle: 'Organizations'
                },
                resolve: {
                    status: [function () {
                        return { edit: false };
                    }],
                    id: [function () {
                        return null;
                    }]
                }
            })
            .state('index.organization_edit', {
                url: "/organizations/:id",
                controller: "OrganizationsEditController as vm",
                templateUrl: "app/components/organizations/edit.html",
                data: {
                    pageTitle: 'Organizations'
                },
                resolve: {
                    status: [function () {
                        return { edit: true };
                    }],
                    id: ['$stateParams', function ($stateParams) {
                        return $stateParams.id;
                    }]
                }
            });

        $urlRouterProvider.otherwise('/index/main');
    }

})();
