import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import User from './User.js';
import { UserStatus, AuthToggle, SetNameForm, WaitingForFightView, MidFightView, FightOverView } from './Components.js';
import * as firebase from 'firebase';


var AppState = {
    WaitingForFight: 0,
    MidFight: 1,
    FightEnded: 2
}

class App extends Component {

    constructor() {
        super();
        this.state = {
            appState: 2,
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
        this.setupDBRefs();
        this.setupUser();
    }

    setupDBRefs() {
        this.rootRef = firebase.database().ref();
        this.rootRef.child("appState").on("value", snap => {
            this.setState({appState: snap.value});
        });
        this.rootRef.child("winner").on("value", snap => {
            this.setState({appState: snap.value});
        });
        this.rootRef.child("tubeman1").child("name").on("value", snap => {
            this.setState({tubeman1: {name: snap.value}});
        });
        this.rootRef.child("tubeman1").child("odds").on("value", snap => {
            this.setState({tubeman1: {odds: snap.value}});
        });
        this.rootRef.child("tubeman2").child("name").on("value", snap => {
            this.setState({tubeman2: {name: snap.value}});
        });
        this.rootRef.child("tubeman2").child("odds").on("value", snap => {
            this.setState({tubeman2: {odds: snap.value}});
        });
    }

    setupUser() {
        this.user = new User();
        this.user.nameChanged = () => this.setState({user: {name: this.user.name}});
        this.user.bankChanged = () => this.setState({user: {bank: this.user.bank}});
        this.user.bet1AmountChanged = () => this.setState({user: {bet1: {amount: this.user.bet1.amount}}});
        this.user.bet1PayoutChanged = () => this.setState({user: {bet1: {payout: this.user.bet1.payout}}});
        this.user.bet2AmountChanged = () => this.setState({user: {bet2: {amount: this.user.bet2.amount}}});
        this.user.bet2PayoutChanged = () => this.setState({user: {bet2: {payout: this.user.bet2.payout}}});
        // this.user.nameChanged = this.nameChanged.bind(this);
        // this.user.bankChanged = this.bankChanged.bind(this);
        // this.user.bet1AmountChanged = this.bet1AmountChanged.bind(this);
        // this.user.bet1PayoutChanged = this.bet1PayoutChanged.bind(this);
        // this.user.bet2AmountChanged = this.bet2AmountChanged.bind(this);
        // this.user.bet2PayoutChanged = this.bet2PayoutChanged.bind(this);
    }

    // nameChanged() {
    //     this.setState( { user: { name: this.user.name } });
    // }

    // bankChanged() {
    //     this.setState( { user: { bank: this.user.bank } });
    // }

    // bet1AmountChanged() {
    //     this.setState( { user: { bet1: { amount: this.user.bet1.amount } } });
    // }

    // bet1PayoutChanged() {
    //     this.setState( { user: { bet1: { payout: this.user.bet1.payout } } });
    // }

    // bet2AmountChanged() {
    //     this.setState( { user: { bet2: { amount: this.user.bet2.amount } } });
    // }

    // bet2PayoutChanged() {
    //     this.setState( { user: { bet2: { payout: this.user.bet2.payout } } });
    // }

    toggleSignIn(e) {
        console.log("toggle");
        if (this.user.isLoggedIn) this.user.signOut();
        else this.user.signIn();
        e.preventDefault();
    }

    setName(name) {
        this.user.setName(name);
    }

    bet(tubeman, amount) {
        this.user(tubeman, amount);
    }

    render() {
        var contentView = null;

        if (this.state.user.isLoggedIn && this.state.user.name === null) {
            contentView = ( <SetNameForm /> );

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
                <AuthToggle isLoggedIn={this.state.user.isLoggedIn} />
            </div>
        );
    }
}

export default App;
