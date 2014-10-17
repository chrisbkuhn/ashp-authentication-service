/// <reference path="js/libs/ember-1.0.0.js" />
/// <reference path="js/libs/ember-model.js" />
App = Ember.Application.create();

App.Router.map(function() {
  // put your routes here
});

App.IndexRoute = Ember.Route.extend({
  model: function() {
      //return ['red', 'yellow', 'blue'];
      return App.InstUser.find();
  }
});

App.InstUser = Ember.Model.extend({
    name: Ember.attr()
    //Username: Ember.attr(),
    //Password: Ember.attr()
});

App.InstUser.adapter = Ember.FixtureAdapter.create(); // Ember.Model.create();

App.InstUser.FIXTURES = [
    { id: 1, name: chris },//, Username: "chris@mail.com", Password: "abcdef" },
    { id: 2, name: john },//, Username: "john@mail.com", Password: "12345" },
    { id: 3, name: sally },//, Username: "sally@mail.com", Password: "qwerty" }
];