/// <reference path="_references.js" />

// 1. Boots up the Ember Application
//App = Ember.Application.create();
App = Ember.Application.create({
    LOG_TRANSITIONS: true,
    LOG_BINDINGS: true,
    LOG_VIEW_LOOKUPS: true,
    LOG_STACKTRACE_ON_DEPRECATION: true,
    LOG_VERSION: true,
    debugMode: true
});

App.SitePath = 'AdminService.svc'; // Dev
//App.SitePath = 'AshpAuth/AdminService.svc'; // Test

/* Authentication */
Ember.Application.initializer({
    name: 'session',

    initialize: function (container, application) {
        App.Session = Ember.Object.extend({
            init: function () {
                this._super();
                this.set('authToken', $.cookie('auth_token'));
                this.set('authAccountId', $.cookie('auth_account'));
            },

            authTokenChanged: function () {
                $.cookie('auth_token', this.get('authToken'));
            }.observes('authToken'),

            authAccountIdChanged: function () {
                var authAccountId = this.get('authAccountId');
                $.cookie('auth_account', authAccountId);
                //if (!Ember.isEmpty(authAccountId)) {
                //    var store = this.get('store');
                //    var account = store.find('account', authAccountId);
                //    this.set('authAccount', account);
                //}
            }.observes('authAccountId'),
            
            isLoggedIn: function () {
                //alert('token: ' + this.get('authToken'));
                return true;
                return this.get('authToken') != '';
            }.property('authToken')
        }).create();
    }
});

Ember.$.ajaxPrefilter(function (options, originalOptions, jqXHR) {
    if (!jqXHR.crossDomain) {
        jqXHR.setRequestHeader('X-AUTHENTICATION-TOKEN', App.Session.get('authToken'));
    }
});





// 2. Define the ROUTER's URL Map .. These need corresponding {{Outlet}} Templates in the index.html file to map to
App.Router.map(function () {
    this.resource("users", function () {
        this.resource('user', { path: '/:id' });
        this.resource('newUser', { path: '/new' });
    });

    this.resource("apps", function () {
        this.resource('app', { path: '/:id' });
        this.resource('newApp', { path: '/new' });
    });

    this.resource("ipranges", function () {
        this.resource('iprange', { path: '/:id' });
        this.resource('newIprange', { path: '/new' });
    });

    this.resource("referers", function () {
        this.resource('referer', { path: '/:id' });
        this.resource('newReferer', { path: '/new' });
    });

    // Authentication
    this.resource('session', function () {
        this.route('new');
        this.route('destroy');
    });

    // Authentication
    this.resource('logs');

});


//**** MODELS

DS.RESTAdapter.reopen({
    namespace: App.SitePath,
    
    ajaxError: function (jqXHR) {

        var error = this._super(jqXHR);
        //alert(JSON.stringify(jqXHR));

        console.log(JSON.stringify(jqXHR));
        //alert(jqXHR.responseText);

        if (jqXHR.status != 200) {

            if (jqXHR.status == 401) {

                alert('Session has expired.');

                App.Session.set('authToken', '');
                App.Session.set('authAccountId', '');
            }
            else {
                alert('An error occurred while communicating with the server');
                if (jqXHR.responseText)
                    alert(jqXHR.responseText);
            }            
        }
    }
});

App.Adapter = DS.RESTAdapter.extend({
});

App.Store = DS.Store.extend({
    revision: 12,
    adapter: 'App.Adapter'
});

App.User = DS.Model.extend({
    username: DS.attr('string'),
    password: DS.attr('string'),
    fullname: DS.attr('string'),

    iprange: DS.hasMany('iprange'),
    referer: DS.hasMany('referer')
});

App.App = DS.Model.extend({
    appcode: DS.attr('string'),
    appname: DS.attr('string'),
    isoffline: DS.attr('boolean', { defaultValue: false }),

    iprange: DS.hasMany('iprange'),
    referer: DS.hasMany('referer')
});

App.Iprange = DS.Model.extend({
    iptype: DS.attr('number'),
    ipmin: DS.attr('string'),
    ipmax: DS.attr('string'),
    slug: DS.attr('string'),

    user: DS.belongsTo('user'),
    app: DS.belongsTo('app')
});

App.Referer = DS.Model.extend({
    url: DS.attr('string'),

    user: DS.belongsTo('user'),
    app: DS.belongsTo('app')
});


// AuthenticatedRoute
App.AuthenticatedRoute = Ember.Route.extend({
    redirectToLogin: function (transition) {
        App.Session.set('attemptedTransition', transition);
        this.transitionTo('session.new');
    },

    beforeModel: function (transition) {
        if (!App.Session.get('authToken')) {
            this.redirectToLogin(transition);
        }
    }
});


App.SessionNewController = Ember.Controller.extend({

    actions: {

        login: function () {

            var self = this;
            var data = this.getProperties('loginOrEmail', 'password');

            if (!Ember.isEmpty(data.loginOrEmail) && !Ember.isEmpty(data.password)) {

                var postData = { session: { login_or_email: data.loginOrEmail, password: data.password } };

                //$.post(App.SitePath + '/session', JSON.stringify(postData))
                $.ajax({
                    type: "POST",
                    url: "/" + App.SitePath + '/session',
                    data: JSON.stringify(postData),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (response) {
                        if (response) {
                            var sessionData = (response.session || {})
                            App.Session.setProperties({
                                authToken: sessionData.token,
                                authAccountId: sessionData.id
                                //authToken: sessionData.auth_token,
                                //authAccountId: sessionData.account_id
                            });

                            var attemptedTransition = App.Session.get('attemptedTransition');

                            if (attemptedTransition) {
                                attemptedTransition.retry();
                                App.Session.set('attemptedTransition', null);
                            } else {
                                self.transitionToRoute('index');
                            }
                        }
                        else {
                            alert("Login Failed. No response returned from Login Service.");
                        }
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        var statusCode = jqXHR.status;

                        if (statusCode == 401)
                            alert('Login Failed. Invalid Username and/or Password.')
                        else
                            alert('Login Failed. Unexpected Error.');
                        //alert('Raw Response:\r\n' + JSON.stringify(jqXHR) + '\r\n' +
                        //      'StatusCode:\r\n' + jqXHR.status + '\r\n' +
                        //      'Status:\r\n' + textStatus + '\r\n' +
                        //      'Error:\r\n' + errorThrown + '\r\n');
                    }
                });
            }
            else {
                alert('Please enter Login Credentials');
            }
        }
    }
});

App.SessionDestroyController = Ember.Controller.extend({

    logout: function () {
        var self = this;
        $.ajax({
            url: "/" + App.SitePath + '/session',
            type: 'DELETE'
        }).always(function (response) {
            App.Session.setProperties({
                authToken: '',
                authAccountId: ''
            });
            self.transitionToRoute('session.new');
        });
    }
});

App.SessionDestroyRoute = Ember.Route.extend({
    renderTemplate: function (controller, model) {
        controller.logout();
    }
});
//App.ApplicationRoute = Ember.Route.extend({
App.ApplicationRoute = App.AuthenticatedRoute.extend({
});

App.ApplicationController = Ember.ObjectController.extend({
    loggedIn: function () { return App.Session.authToken != ''; }.property()
});

//App.IndexRoute = Ember.Route.extend({
App.IndexRoute = App.AuthenticatedRoute.extend({
});

// Define Routes' Model Hooks that Map URL to a Model
//App.UsersRoute = Ember.Route.extend({
App.UsersRoute = App.AuthenticatedRoute.extend({ 
    model: function () {
        var store = this.get('store');
        var allUsers = store.findAll('user');
        return allUsers;
    }
});

App.UsersController = Ember.ArrayController.extend({
    actions: {
        new: function () {
            this.get("target").transitionTo("newUser");
        },

        sortBy: function(property) {            
            this.set('sortProperties', [property]);
            this.set('sortAscending', !this.get('sortAscending'));
        }
    },

    searchText: '',

    filteredContent: function () {

        var content = this.get('content');
        var search = this.get('searchText').trim().toLowerCase();

        if (search == '') { 
            return content;
        } else {
            return content.filter(function (item) {
                return item.get('username').toLowerCase().indexOf(search) !== -1 ||
                       item.get('password').toLowerCase().indexOf(search) !== -1 ||
                       item.get('fullname').toLowerCase().indexOf(search) !== -1;
            });
        }

    }.property('content', 'searchText', 'content.@each')
});

// Define Routes' Model Hooks that Map URL to a Model
//App.UserRoute = Ember.Route.extend({
App.UserRoute = App.AuthenticatedRoute.extend({
    model: function (user) {
        var store = this.get('store');
        var existingUser = store.find('user', user.id);
        return existingUser;
    },

    //serialize: function (model, params) {
    //    // this will make the URL `/users/username`
    //    return { username: model.username };
    //}
});

App.UserController = Ember.ObjectController.extend({
    actions: {
        save: function (user) {
            user.save()
                .then(function () {
                    console.log("User.save :: ok")
                }).fail(function (error) {
                    console.log(JSON.stringify(error));

                    this.store.unloadAll('user');
                    user.reload();

                    if (App.Session.authToken == '') {
                        alert('Session Expired. Please Login again.');
                        this.get("target").transitionTo("index");
                    }
                });

            alert('User Saved!');
            this.get("target").transitionTo("users");
        },

        delete: function (user) {
            if (confirm('Are you sure?')) {
                user.deleteRecord();
                user.save()
                    .then(function () {
                        console.log("User.delete :: ok")
                    }).fail(function (error) {
                        console.log(JSON.stringify(error));

                        this.store.unloadAll('user');
                        user.reload();

                        if (App.Session.authToken == '') {
                            alert('Session Expired. Please Login again.');
                            this.get("target").transitionTo("index");
                        }
                    });

                this.get("target").transitionTo("users");
            }
        },

        cancel: function () {
            this.get("target").transitionTo("users");
        }
    }
});

//App.NewUserRoute = Ember.Route.extend({
App.NewUserRoute = App.AuthenticatedRoute.extend({
    model: function (params) {
        var store = this.get('store');
        var newUser = store.createRecord('user');
        return newUser;
    },

    //serialize: function(model) {
    //    // this will make the URL `/users/username`
    //    return { username: model.Username };
    //}
});

App.NewUserController = Ember.ObjectController.extend({
    actions: {
        save: function (user) {

            user.save()
                .then(function () {
                    console.log("User.save :: ok")
                }).fail(function (error) {
                    console.log(JSON.stringify(error));

                    this.store.unloadAll('user');
                    user.reload();

                    if (App.Session.authToken == '') {
                        alert('Session Expired. Please Login again.');
                        this.get("target").transitionTo("index");
                    }
                });

            alert('User Saved!');
            this.get("target").transitionTo("users");
        }
    }
});


//App.AppsRoute = Ember.Route.extend({
App.AppsRoute = App.AuthenticatedRoute.extend({
    model: function () {
        var store = this.get('store');
        var allApps = store.findAll('app');
        return allApps;
    }
});

App.AppsController = Ember.ArrayController.extend({
    actions: {
        new: function () {
            this.get("target").transitionTo("newApp");
        },

        sortBy: function (property) {
            this.set('sortProperties', [property]);
            this.set('sortAscending', !this.get('sortAscending'));
        }
    },

    searchText: '',

    filteredContent: function () {

        var content = this.get('content');
        var search = this.get('searchText').trim().toLowerCase();

        if (search == '') {
            return content;
        } else {
            return content.filter(function (item) {
                return item.get('appcode').toLowerCase().indexOf(search) !== -1 ||
                       item.get('appname').toLowerCase().indexOf(search) !== -1 ;
            });
        }

    }.property('content', 'searchText', 'content.@each')
});


//App.AppRoute = Ember.Route.extend({
App.AppRoute = App.AuthenticatedRoute.extend({
    model: function (params) {
        var store = this.get('store');
        var existingApp = store.find('app', params.id);
        return existingApp;
    }
});

App.AppController = Ember.ObjectController.extend({
    actions: {
        save: function (app) {
            app.save()
                .then(function () {
                    console.log("App.save :: ok")
                }).fail(function (error) {
                    console.log(JSON.stringify(error));

                    this.store.unloadAll('app');
                    app.reload();

                    if (App.Session.authToken == '') {
                        alert('Session Expired. Please Login again.');
                        this.get("target").transitionTo("index");
                    }
                });

            alert('Application Saved!');
            this.get("target").transitionTo("apps");
        },

        delete: function (app) {
            if (confirm('Are you sure?')) {
                app.deleteRecord();
                app.save()
                    .then(function () {
                        console.log("App.delete :: ok")
                    }).fail(function (error) {
                        console.log(JSON.stringify(error));

                        this.store.unloadAll('app');
                        app.reload();

                        if (App.Session.authToken == '') {
                            alert('Session Expired. Please Login again.');
                            this.get("target").transitionTo("index");
                        }
                    });

                this.get("target").transitionTo("apps");
            }
        }
    }
});

//App.NewAppRoute = Ember.Route.extend({
App.NewAppRoute = App.AuthenticatedRoute.extend({
    model: function (params) {
        var store = this.get('store');
        var newApp = store.createRecord('app');
        return newApp;
    },
});

App.NewAppController = Ember.ObjectController.extend({
    actions: {
        save: function (app) {

            app.save()
                .then(function () {
                    console.log("App.save :: ok")
                }).fail(function (error) {
                    console.log(JSON.stringify(error));

                    this.store.unloadAll('app');
                    app.reload();

                    if (App.Session.authToken == '') {
                        alert('Session Expired. Please Login again.');
                        this.get("target").transitionTo("index");
                    }
                });

            alert('Application Saved!');
            this.get("target").transitionTo("apps");
        }
    }
});




// ** IP Range


//App.IprangesRoute = Ember.Route.extend({
App.IprangesRoute = App.AuthenticatedRoute.extend({
    model: function () {
        var store = this.get('store');
        var ipRanges = store.findAll('iprange');
        return ipRanges;
    }
});

App.IprangesController = Ember.ArrayController.extend({
    actions: {
        new: function () {
            this.get("target").transitionTo("newIprange");
        },

        sortBy: function (property) {
            this.set('sortProperties', [property]);
            this.set('sortAscending', !this.get('sortAscending'));
        }
    },

    searchText: '',

    filteredContent: function () {

        var content = this.get('content');
        var search = this.get('searchText').trim().toLowerCase();

        if (search == '') {
            return content;
        } else {
            return content.filter(function (item) {
                return item.get('ipmin').toLowerCase().indexOf(search) !== -1 ||
                       item.get('ipmax').toLowerCase().indexOf(search) !== -1 ||
                       item.get('user.username').toLowerCase().indexOf(search) !== -1 ||
                       item.get('app.appcode').toLowerCase().indexOf(search) !== -1;
            });
        }

    }.property('content', 'searchText', 'content.@each')
});

//App.IprangeRoute = Ember.Route.extend({
App.IprangeRoute = App.AuthenticatedRoute.extend({
    model: function (params) {
        var store = this.get('store');
        var existingIpRange = store.find('iprange', params.id);
        return existingIpRange;
    }
});

App.IprangeController = Ember.ObjectController.extend({
    
    iptypes: function () {
        var types =
            [
                { 'id': 4, 'type': 4 },
                { 'id': 6, 'type': 6 }
            ];
        return types;

    }.property(),

    applications: function () {
        var allApps = this.get('store').findAll('app')
        return allApps;
    }.property(),

    users: function () {
        var allUsers = this.get('store').findAll('user')
        return allUsers;
    }.property(),

    actions: {
        save: function (ipRange) {

            ipRange.save()
                .then(function () {
                    console.log("Iprange.save :: ok")
                }).fail(function (error) {
                    console.log(JSON.stringify(error));

                    this.store.unloadAll('iprange');
                    ipRange.reload();

                    if (App.Session.authToken == '') {
                        alert('Session Expired. Please Login again.');
                        this.get("target").transitionTo("index");
                    }
                });

            alert('IP Range Saved!');
            this.get("target").transitionTo("ipranges");
        },

        delete: function (ipRange) {
            if (confirm('Are you sure?')) {
                ipRange.deleteRecord();
                ipRange.save()
                    .then(function () {
                        console.log("Iprange.delete :: ok")
                    }).fail(function (error) {
                        console.log(JSON.stringify(error));

                        this.store.unloadAll('iprange');
                        ipRange.reload();

                        if (App.Session.authToken == '') {
                            alert('Session Expired. Please Login again.');
                            this.get("target").transitionTo("index");
                        }
                    });

                this.get("target").transitionTo("ipranges");
            }
        }
    }
});


//App.NewIprangeRoute = Ember.Route.extend({
App.NewIprangeRoute = App.AuthenticatedRoute.extend({
    model: function (params) {
        var store = this.get('store');
        var newApp = store.createRecord('iprange');
        return newApp;
    }
});

App.NewIprangeController = Ember.ObjectController.extend({
    
    iptypes: function () {
        var types =
            [
                { 'id': 4, 'type': 4 },
                { 'id': 6, 'type': 6 }
            ];
        return types;
    }.property(),

    applications: function () {
        var allApps = this.get('store').findAll('app')
        return allApps;
    }.property(),

    users: function () {
        var allUsers = this.get('store').findAll('user')
        return allUsers;
    }.property(),

    actions: {
        save: function (iprange) {

            iprange.save()
                .then(function () {
                    console.log("Iprange.save :: ok")
                }).fail(function (error) {
                    console.log(JSON.stringify(error));

                    this.store.unloadAll('iprange');
                    ipRange.reload();

                    if (App.Session.authToken == '') {
                        alert('Session Expired. Please Login again.');
                        this.get("target").transitionTo("index");
                    }
                });

            alert('IP Range Saved!');
            this.get("target").transitionTo("ipranges");
        }
    }
});


// ** Referer

//App.ReferersRoute = Ember.Route.extend({
App.ReferersRoute = App.AuthenticatedRoute.extend({
    model: function () {
        var store = this.get('store');
        var allReferers = store.findAll('referer');
        return allReferers;
    }
});

App.ReferersController = Ember.ArrayController.extend({
    actions: {
        new: function () {
            this.get("target").transitionTo("newReferer");
        },

        sortBy: function (property) {
            this.set('sortProperties', [property]);
            this.set('sortAscending', !this.get('sortAscending'));
        }
    },

    searchText: '',

    filteredContent: function () {

        var content = this.get('content');
        var search = this.get('searchText').trim().toLowerCase();

        if (search == '') {
            return content;
        } else {
            return content.filter(function (item) {
                return item.get('url').toLowerCase().indexOf(search) !== -1 ||
                       item.get('user.username').toLowerCase().indexOf(search) !== -1 ||
                       item.get('app.appcode').toLowerCase().indexOf(search) !== -1;
            });
        }

    }.property('content', 'searchText', 'content.@each')
});

//App.RefererRoute = Ember.Route.extend({
App.RefererRoute = App.AuthenticatedRoute.extend({
    model: function (params) {
        var existingReferer = this.get('store').find('referer', params.id);
        return existingReferer;
    }
});

App.RefererController = Ember.ObjectController.extend({

    applications: function () {
        var allApps = this.get('store').findAll('app')
        return allApps;
    }.property(),

    users: function () {
        var allUsers = this.get('store').findAll('user')
        return allUsers;
    }.property(),

    actions: {
        save: function (referer) {

            referer.save()
                .then(function () {
                    console.log("Referer.save :: ok")
                }).fail(function (error) {
                    console.log(JSON.stringify(error));

                    this.store.unloadAll('referer');
                    referer.reload();

                    if (App.Session.authToken == '') {
                        alert('Session Expired. Please Login again.');
                        this.get("target").transitionTo("index");
                    }
                });

            alert('Referer Saved!');
            this.get("target").transitionTo("referers");
        },

        delete: function (referer) {
            if (confirm('Are you sure?')) {
                referer.deleteRecord();
                referer.save()
                    .then(function () {
                        console.log("Referer.delete :: ok")
                    }).fail(function (error) {
                        console.log(JSON.stringify(error));

                        this.store.unloadAll('referer');
                        referer.reload();

                        if (App.Session.authToken == '') {
                            alert('Session Expired. Please Login again.');
                            this.get("target").transitionTo("index");
                        }
                    });

                this.get("target").transitionTo("referers");
            }
        }
    }
});


//App.NewRefererRoute = Ember.Route.extend({
App.NewRefererRoute = App.AuthenticatedRoute.extend({
    model: function (params) {
        var store = this.get('store');
        var newApp = store.createRecord('referer');
        return newApp;
    }
});

App.NewRefererController = Ember.ObjectController.extend({
    
    applications: function () {
        var allApps = this.get('store').findAll('app')
        return allApps;
    }.property(),

    users: function () {
        var allUsers = this.get('store').findAll('user')
        return allUsers;
    }.property(),

    actions: {
        save: function (referer) {

            referer.save()
                .then(function () {
                    console.log("Referer.save :: ok")
                }).fail(function (error) {
                    console.log(JSON.stringify(error));

                    this.store.unloadAll('referer');
                    referer.reload();

                    if (App.Session.authToken == '') {
                        alert('Session Expired. Please Login again.');
                        this.get("target").transitionTo("index");
                    }
                });

            alert('Referer Saved!');
            this.get("target").transitionTo("referers");
        }
    }
});