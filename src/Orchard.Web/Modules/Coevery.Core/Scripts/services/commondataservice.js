﻿'use strict';

define(['core/app/detourService'], function (detour) {

    //This handles retrieving data and is used by controllers. 3 options (server, factory, provider) with 
    //each doing the same thing just structuring the functions/data differently.

    //Although this is an AngularJS factory I prefer the term "service" for data operations
    detour.registerFactory([
        'commonDataService',
        ['$rootScope', '$resource', function ($rootScope, $resource) {
            return $resource('api/CoeveryCore/Common/:contentType:contentId:PageSize:Page:ViewId',
                { contentType: '@contentType' },
                { contentId: '@ContentId' },
                { pageSize: '@PageSize' },
                { viewId: '@ViewId' },
                { page: '@Page' },
                { update: { method: 'PUT' } });
        }]
    ]);
});