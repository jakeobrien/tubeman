import React from 'react';
import ReactDOM from 'react-dom';
import App from './App';
import './index.css';
import * as firebase from 'firebase';

var config = {
    apiKey: "AIzaSyBCrH3gdhCPNuL8ATHcl88JCYNyfquaz30",
    authDomain: "tubeman-677a6.firebaseapp.com",
    databaseURL: "https://tubeman-677a6.firebaseio.com",
    projectId: "tubeman-677a6",
    storageBucket: "tubeman-677a6.appspot.com",
    messagingSenderId: "643336832336"
  };
firebase.initializeApp(config);

ReactDOM.render(
  <App />,
  document.getElementById('root')
);
