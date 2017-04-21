import Auth from './Auth.js';
import * as firebase from 'firebase';

const DefaultUser = {
    name: null,
    bank: 100,
    bet1: {
        amount: 0,
        payout: 0 
    },
    bet2: {
        amount: 0,
        payout: 0
    }
};

class User {

    constructor() {
        this.auth = new Auth();
        this.auth.stateChanged = this.authStateChanged.bind(this);
        this.isLoggedIn = this.auth.isLoggedIn;
        this.name = null;
        this.bank = 0;
        this.bet1 = {
            amount: 0,
            payout: 0
        };
        this.bet2 = {
            amount: 0,
            payout: 0
        };
        this.rootRef = firebase.database().ref();
        this.usersRef = this.rootRef.child("users");
        this.userRef = null;
    }

    signIn() {
        this.auth.signIn();
    }

    signOut() {
        this.auth.signOut();
    }

    setName(name) {
        this.userRef.child("name").set(name);
    }

    bet(tubeman, amount) {
        if (this.bank < amount) return;
        var bet = this.bet1;
        var betName = "bet1";
        if (tubeman === 2) {
            bet = this.bet2;
            betName = "bet2";
        }
        this.userRef.child("bank").set(this.bank - amount);
        this.userRef.child(betName).child("amount").set(bet.amount + amount);
    }

    authStateChanged() {
        this.isLoggedIn = this.auth.isLoggedIn;
        if (this.isLoggedInChanged) this.isLoggedInChanged();
        if (this.isLoggedIn) {
            this.setupUser();
        } else {
            this.userRef = null;
            this.name = null;
            if (this.nameChanged) this.nameChanged();
            this.bank = null;
            if (this.bankChanged) this.bankChanged();
            this.bet1.amount = 0;
            if (this.bet1Amount) this.bet1Amount();
            this.bet1.payout = 0;
            if (this.bet1Payout) this.bet1Payout();
            this.bet2.amount = 0;
            if (this.bet2Amount) this.bet2Amount();
            this.bet2.payout = 0;
            if (this.bet2Payout) this.bet2Payout();
        }
    }

    setupUser() {
        this.usersRef.once("value", snap => {
            if (!snap.hasChild(this.auth.uid)) {
                this.usersRef.child(this.auth.uid).set(DefaultUser);
            }
            this.userRef = this.usersRef.child(this.auth.uid);
            this.userRef.child("name").on("value", snap => {
                this.name = snap.val();
                if (this.nameChanged) this.nameChanged();
            });
            this.userRef.child("bank").on("value", snap => {
                this.bank = snap.val();
                if (this.bankChanged) this.bankChanged();
            });
            this.userRef.child("bet1").child("amount").on("value", snap => {
                this.bet1.amount = snap.val();
                if (this.bet1Amount) this.bet1Amount();
            });
            this.userRef.child("bet1").child("payout").on("value", snap => {
                this.bet1.payout = snap.val();
                if (this.bet1Payout) this.bet1Payout();
            });
            this.userRef.child("bet2").child("amount").on("value", snap => {
                this.bet2.amount = snap.val();
                if (this.bet2Amount) this.bet2Amount();
            });
            this.userRef.child("bet2").child("payout").on("value", snap => {
                this.bet2.payout = snap.val();
                if (this.bet2Payout) this.bet2Payout();
            });
        });
    }


}

export default User;