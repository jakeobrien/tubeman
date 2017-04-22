import React, { Component } from 'react';
// import logo from './logo.svg';
import './App.css';
import User from './User.js';
import { UserStatus, AuthToggle, SetNameForm, WaitingForFightView, MidFightView, FightOverView } from './Components.js';
import * as firebase from 'firebase';

var AppState = {
    WaitingForFight: 0,
    MidFight: 1,
    FightEnded: 2
}
// {
//   "rules": {
//     "users": {
//       "$user_id": {
//         ".read": "$user_id === auth.uid",
//         ".write": "$user_id === auth.uid"
//       }
//     },
//     ".read": "true",
//     ".write": "auth != null"
//   }
// }
class App extends Component {

    constructor() {
        super();
        this.toggleSignIn = this.toggleSignIn.bind(this);
        this.nameChanged = this.nameChanged.bind(this);
        this.setName = this.setName.bind(this);
        this.state = {
            tentativeName: null,
            appState: 0,
            winner: 0,
            tubeman1: {
                name: "green",
                odds: "1:1"
            },
            tubeman2:  {
                name: "blue",
                odds: "1:1"
            },
            user: {
                name: "t",
                bank: 0,
                isLoggedIn: true,
                bet1: {
                    amount: 0,
                    payout: 0
                },
                bet2: {
                    amount: 0,
                    payout: 0
                }
            }
        };
    }

    componentDidMount() {
        this.setupUser();
        this.setupDBRefs();
    }

    setupDBRefs() {
        this.rootRef = firebase.database().ref();
        this.rootRef.child("appState").on("value", snap => {
            this.setState({appState: snap.val()});
        });
        this.rootRef.child("winner").on("value", snap => {
            this.setState({winner: snap.val()});
        });
        this.rootRef.child("tubeman1").child("name").on("value", snap => {
            this.setState((prevState, props) => ({
                tubeman1: {...prevState.tubeman1, name: snap.val()}}
            ));
        });
        this.rootRef.child("tubeman1").child("odds").on("value", snap => {
            this.setState((prevState, props) => ({
                tubeman1: {...prevState.tubeman1, odds: snap.val()}}
            ));
        });
        this.rootRef.child("tubeman2").child("name").on("value", snap => {
            this.setState((prevState, props) => ({
                tubeman2: {...prevState.tubeman2, name: snap.val()}}
            ));
        });
        this.rootRef.child("tubeman2").child("odds").on("value", snap => {
            this.setState((prevState, props) => ({
                tubeman2: {...prevState.tubeman2, odds: snap.val()}}
            ));
        });
    }

    setupUser() {
        this.user = new User();
        this.user.isLoggedInChanged = () => this.setState((prevState, props) => ({
            user: {...prevState.user, isLoggedIn: this.user.isLoggedIn}
        }));
        this.user.nameChanged = () => this.setState((prevState, props) => ({
            user: {...prevState.user, name: this.user.name}
        }));
        this.user.bankChanged = () => this.setState((prevState, props) => ({
            user: {...prevState.user, bank: this.user.bank}
        }));
        this.user.bet1AmountChanged = () => this.setState((prevState, props) => ({
            user: {...prevState.user, bet1: {...prevState.user.bet1, amount: this.user.bet1.amount}}
        }));
        this.user.bet1PayoutChanged = () => this.setState((prevState, props) => ({
            user: {...prevState.user, bet1: {...prevState.user.bet1, payout: this.user.bet1.payout}}
        }));
        this.user.bet2AmountChanged = () => this.setState((prevState, props) => ({
            user: {...prevState.user, bet2: {...prevState.user.bet2, amount: this.user.bet2.amount}}
        }));
        this.user.bet2PayoutChanged = () => this.setState((prevState, props) => ({
            user: {...prevState.user, bet2: {...prevState.user.bet2, payout: this.user.bet2.payout}}
        }));
    }

    toggleSignIn(event) {
        if (this.user.isLoggedIn) this.user.signOut();
        else this.user.signIn();
        event.preventDefault();
    }

    nameChanged(event) {
        this.setState({
            tentativeName: event.target.value
        });
    }

    setName(event) {
        this.user.setName(this.state.tentativeName);
        event.preventDefault();
    }

    bet(tubeman, amount) {
        this.user(tubeman, amount);
    }

    render() {
        var contentView = null;

        if (this.state.user.isLoggedIn && (this.state.user.name === null || this.state.user.name.length == 0)) {
            contentView = ( <SetNameForm onNameChanged={this.nameChanged} onSubmit={this.setName} /> );

        } else if (this.state.appState === AppState.WaitingForFight) {
            contentView = ( <WaitingForFightView /> );

        } else if (this.state.appState === AppState.MidFight) {
            contentView = ( <MidFightView 
                                tubeman1={this.state.tubeman1} 
                                tubeman2={this.state.tubeman2} 
                                user={this.state.user} /> );

        } else if (this.state.appState === AppState.FightEnded) {
            var winner = null;
            if (this.state.winner === 1) winner = this.state.tubeman1;
            else if (this.state.winner === 2) winner = this.state.tubeman2;
            contentView = ( <FightOverView 
                                tubeman1={this.state.tubeman1} 
                                tubeman2={this.state.tubeman2} 
                                winner={winner} 
                                user={this.state.user} /> );
        } 

        return (
            <div className="App">
                <UserStatus user={this.state.user} />
                {contentView}
                <AuthToggle isLoggedIn={this.state.user.isLoggedIn} onSubmit={this.toggleSignIn} />
            </div>
        );
    }
}

export default App;
