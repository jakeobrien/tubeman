import React, { Component } from 'react';
import logo from './logo.svg';
import './App.css';
import * as firebase from 'firebase';

class UserStatus extends Component {
    render() {
        if (!this.props.user.isloggedin) return ( <div /> );
        return (
            <div>
                <span>{this.props.user.name}</span>
                <span>${this.props.user.bank}</span>
            </div>
        );
    }
}

class AuthToggle extends Component {
    render() {
        var buttonText = this.props.isLoggedIn ? "Sign Out" : "Sign In";
        return (
            <div><button>{buttonText}</button></div>
        );
    }
}

class SetNameForm extends Component {
    render() {
        return (
            <div>
                <form>
                    <input type="text" placeholder="Enter name..." />
                    <submit>Go</submit>
                </form>
            </div>
        );
    }
}

class WaitingForFightView extends Component {
    render() {
        return (
            <div>
                <div>Waiting For Fight To Start</div>
            </div>
        );
    }
}

class MidFightView extends Component {
    render() {
        return (
            <div>
                <div>Your bets</div>
                <table>
                    <tr>
                        <MidFightBetView tubeman={this.props.tubeman1} userbet={this.props.user.bet1} />
                        <MidFightBetView tubeman={this.props.tubeman2} userbet={this.props.user.bet2} />
                    </tr>
                </table>
            </div>
        );
    }
}

class MidFightBetView extends Component {
    render() {
        return (
            <td>
                <div>{this.props.tubeman.name}</div>
                <div>Current odds: {this.props.tubeman.odds}</div>
                <div>Current bet: ${this.props.userbet.amount}</div>
                <BetForm />
            </td>
        );
    }
}

class BetForm extends Component {
    render() {
        return (
            <div>
                <form>
                    $<input />
                    <submit>Bet</submit>
                </form>
            </div>
        );
    }
}

class FightOverView extends Component {
    render() {
        return (
            <div>
                <div>{this.props.winner.name} won</div>
                <div>at {this.props.winner.odds} odds</div>
                <div>Your Bets</div>
                <table>
                    <tr>
                        <FightOverResult tubeman={this.props.tubeman1} userbet={this.props.user.bet1} />
                        <FightOverResult tubeman={this.props.tubeman2} userbet={this.props.user.bet2} />
                    </tr>
                </table>
            </div>
        );
    }
}

class FightOverView extends Component {
    render() {
        return (
            <td>
                <table>
                    <tr>
                        <td>{this.props.tubeman.name}:</td>
                        <td>${this.props.userbet.amount}</td>
                    </tr>
                    <tr>
                        <td>Odds:</td>
                        <td>{this.props.tubeman.odds}</td>
                    </tr>
                    <tr>
                        <td>Pays:</td>
                        <td>${this.props.userbet.payout}</td>
                    </tr>
                </table>
            </td>
        );
    }
}

var AppState = {
    WaitingForFight: 1,
    MidFight: 2,
    FightEnded: 3
}

class MainView extends Component {

    constructor() {
        super();
        // this.signIn = this.signIn.bind(this);
    }


    render() {
        var contentView = null;
        if (this.props.user.isLoggedIn && this.props.user.name === null) {
            contentView = ( <SetNameForm /> );
        } else if (this.props.appState == AppState.WaitingForFight) {
            contentView = ( <WaitingForFightView /> );
        } else if (this.props.appState == AppState.MidFight) {
            contentView = ( <MidFightView tubeman1={this.props.tubeman1} tubeman2={this.props.tubeman2} user={this.props.user} /> );
        } else if (this.props.appState == AppState.FightEnded) {
            contentView = ( <FightOverView tubeman1={this.props.tubeman1} tubeman2={this.props.tubeman2} user={this.props.user} /> );
        } 
        return (
            <div>
                <UserStatus user={this.props.user} />
                {contentView}
                <AuthToggle />
            </div>
        );
    }
}

class App extends Component {

    constructor() {
        super();
        this.data = {
            appState: 0,
            winner: {
                name: "green",
                odds: "1:2"
            },
            tubeman1: {
                name: "blue",
                odds: "2:1"
            },
            tubeman2: {
                name: "green",
                odds: "1:2"
            },
            user: {
                name: null,
                bank: 100,
                isLoggedIn: true,
                bet1: {
                    amount: 10,
                    payout: 20
                },
                bet2: {
                    amount: 0,
                    payout: 0
                }
            }
        };
    }

    render() {
        return (
            <MainView appState={this.data.appState} tubeman1={this.data.tubeman1} tubeman2={this.data.tubeman2} winner={this.data.winner} user={this.data.user} />
        );
    }
}

export default App;
