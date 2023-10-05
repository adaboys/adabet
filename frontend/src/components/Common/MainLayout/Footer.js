import classNames from 'classnames';
import Link from 'next/link';

import IconDiscord from '@assets/icons/icon-discord.svg';
import IconFacebook from '@assets/icons/icon-facebook.svg';
import IconInstagram from '@assets/icons/icon-instagram.svg';
import IconTwitter from '@assets/icons/icon-twitter.svg';
import IconYoutube from '@assets/icons/icon-youtube.svg';

import styles from './Footer.module.scss';

const Footer = () => {
    return (
        <footer className={styles.footer}>
            <div className="container">
                <div className={classNames('row', styles.content)}>
                    <div className={classNames('col-4', styles.logo)}>
                        <img src="/images/full-logo.png" alt="" />
                    </div>
                    <div className="col-5">
                        <div className={styles.footerMenu}>
                            <nav className={styles.menuCol}>
                                <h3>Bet</h3>
                                <Link href="#">Sports Home</Link>
                                <Link href="#">Live</Link>
                                <Link href="#">Rules</Link>
                            </nav>
                            <nav className={styles.menuCol}>
                                <h3>Promo</h3>
                                <Link href="#">VIP Club</Link>
                                <Link href="#">Promotions</Link>
                                <Link href="#">Affiliate</Link>
                            </nav>
                            <nav className={styles.menuCol}>
                                <h3>Support</h3>
                                <Link href="#">Help Center</Link>
                                <Link href="#">FAQ</Link>
                                <Link href="#">Pivate Policy</Link>
                                <Link href="#">Term Of Service</Link>
                            </nav>
                        </div>
                    </div>
                    <div className={classNames('col-3', styles.social)}>
                        <h4>Join our community</h4>
                        <div className={styles.list}>
                            <IconDiscord />
                            <IconFacebook />
                            <IconInstagram />
                            <IconTwitter />
                            <IconYoutube />
                        </div>
                    </div>
                </div>
            </div>
            <div className={styles.copyright}>Copyright © 2022. All Rights Reserved By Adabet</div>
        </footer>
    )
}

export default Footer;