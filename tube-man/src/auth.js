
class Auth {
    // constructor() {
    //     super();
    //     this.state = {
    //         startMoney: 100,
    //         uid: null,
    //         isLoggedIn: 0
    //     };
    //     this.signIn = this.signIn.bind(this);
    //     this.bet = this.bet.bind(this);
    //     this.signOut = this.signOut.bind(this);
    // }

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
        if (user && this.uid === null) {
            this.uid = user.uid;
            this.isLoggedIn = 1;
            this.getUserRef();
        } else if (user === null && this.uid != null) {
            this.uid = null;
            this.isLoggedIn = -1;
        }
    }

    signIn() {
        firebase.auth().signInAnonymously().catch(function (error) {
            console.log(error.code + "  " + error.message);
        });
    }

    signOut() {
        firebase.auth().signOut();
    }

    bindToDB() {
        this.rootRef = firebase.database().ref(),
        this.startMoneyRef = firebase.database().ref().child("startMoney")
        this.startMoneyRef.on("value", snap => {
            this.startMoney = snap.val();
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
                this.money = snap.val();
            });
        });
    }

    bet() {
        this.moneyRef.set(this.state.money - 10);
    }

}