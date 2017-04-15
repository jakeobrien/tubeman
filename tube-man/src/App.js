import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import * as firebase from 'firebase';

class App extends Component {

    constructor() {
        super();
        this.state = {
            startMoney: 100,
            uid: null,
            errorCode: 0,
            errorMessage: "",
            isLoggedIn: 0,
            status: 0
        };
        this.signIn = this.signIn.bind(this);
        this.bet = this.bet.bind(this);
        this.signOut = this.signOut.bind(this);
    }

    componentDidMount() {
        this.bindToDB();
        this.auth();
    }

    auth() {
        this.onAuth(firebase.auth().currentUser);
        firebase.auth().onAuthStateChanged(this.onAuthChanged.bind(this));
    }

    onAuthChanged(user) {
        this.onAuth(user);
    }

    onAuth(user) {
        this.setStateForUser(user);
    }

    setStateForUser(user) {
        if (user && this.state.uid === null) {
            this.setState({
                uid: user.uid,
                isLoggedIn: 1
            });
            this.getUserRef();
        } else if (user === null && this.state.uid != null) {
            this.setState({
                uid: null,
                isLoggedIn: -1
            });
        }
    }

    signIn() {
        this.setState({
            status: 5
        });
        firebase.auth().signInAnonymously().catch(function (error) {
            this.setState({
                errorCode: error.code,
                errorMessage: error.message,
                status: 6
            });
        });
    }

    signOut() {
        firebase.auth().signOut();
    }

    bindToDB() {
        this.rootRef = firebase.database().ref(),
        this.startMoneyRef = firebase.database().ref().child("startMoney")
        this.startMoneyRef.on("value", snap => {
            this.setState({
                startMoney: snap.val()
            });
        });
    }

    getUserRef() {
        const usersRef = this.rootRef.child("users");
        usersRef.once("value", snap => {
            if (!snap.hasChild(this.state.uid)) {
                console.log("make new");
                usersRef.child(this.state.uid).set({
                    money: this.state.startMoney
                });
            }
            this.userRef = usersRef.child(this.state.uid);
            this.moneyRef = usersRef.child(this.state.uid).child("money");
            this.moneyRef.on("value", snap => {
                this.setState({
                    money: snap.val()
                });
            });
        });
    }

    bet() {
        this.moneyRef.set(this.state.money - 10);
    }

    renderMenu() {
        if (this.state.isLoggedIn == 1) {
            return (
                <div>
                <button onClick={this.bet}>Bet</button>
                <button onClick={this.signOut}>Sign Out</button>
                </div>
            );
        } else {
            return (
                <div>
                <button onClick={this.signIn}>Sign In</button>
                </div>
            );
        }
    }

    render() {
        return (
            <div className="App">
                <p>User ID: {this.state.uid}</p>
                <h3>Money: ${this.state.money}</h3>
                {this.renderMenu()}
                <p>{this.state.isLoggedIn} {this.state.status} {this.state.errorCode} {this.state.errorMessage}</p>
            </div>
        );
    }
}

export default App;
