import classNames from 'classnames';
import Link from 'next/link';
import { paths } from '@constants';

import SubscribeForm from './SubscribeForm';

// import DiscordIcon from '@assets/icons/discord-social.svg';
// import TelegramIcon from '@assets/icons/telegram.svg';
// import YoutubeIcon from '@assets/icons/youtube-social.svg';
// import FacebookIcon from '@assets/icons/facebook-social.svg';
// import TwitterIcon from '@assets/icons/twitter-social.svg';

import styles from './Footer.module.scss';
import { FormattedMessage } from 'react-intl';

const Footer = () => {
    return (
        <footer className={styles.footer}>
            {/* <div className="container">
                <div className={classNames('row', styles.nav)}>
                    <div className={styles.logo}>
                        <Link href={paths.home}>
                            <img src="/images/logo.png" alt="IronSky" />
                        </Link>
                    </div>
                    <ul className={styles.menu}>
                        <li className={styles.menuItem}>
                            <Link href={paths.home}>
                                <a><FormattedMessage key="dashboard" defaultMessage="Dashboard" /></a>
                            </Link>
                        </li>
                        <li className={styles.menuItem}>
                            <a href="https://www.ironsky.info/updates" target="_blank">
                                <FormattedMessage key="news" defaultMessage="News" tagName="a" />
                            </a>
                        </li>
                        <li className={styles.menuItem}>
                            <a href="https://ironsky.gitbook.io/ironsky-game-ver1.0.0/" target="_blank">
                                <FormattedMessage key="whitepaper" defaultMessage="Whitepaper" tagName="a" />
                            </a>
                        </li>
                        <li className={styles.menuItem}>
                            <Link href={paths.termsOfService}>
                                <a><FormattedMessage key="temrOfUse" defaultMessage="Terms of Use"/></a>
                            </Link>
                        </li>
                        <li className={styles.menuItem}>
                            <Link href={paths.home}>
                                <FormattedMessage key="cookiePolicy" defaultMessage="Cookie Policy" tagName="a" />
                            </Link>
                        </li>
                        <li className={styles.menuItem}>
                            <Link href={paths.home}>
                                <FormattedMessage key="privacyPolicy" defaultMessage="Privacy Policy" tagName="a" />
                            </Link>
                        </li>
                        <li className={styles.menuItem}>
                            <Link href={paths.faqs}>
                                <FormattedMessage key="faqs" defaultMessage="FAQs" tagName="a" />
                            </Link>
                        </li>
                    </ul>
                    <div className={styles.marketing}>
                        <FormattedMessage key="joinOurCommunity" defaultMessage="Join our community" tagName="p" />
                        <div className={styles.social}>
                            <a href="https://discord.gg/wknaqJk7tp" target="_blank"><DiscordIcon/></a>
                            <a href="https://t.me/ironskygame" target="_blank"><TelegramIcon/></a>
                            <a href="https://www.youtube.com/watch?v=Tf1cKXB_aCs" target="_blank"><YoutubeIcon/></a>
                            <a href="https://www.facebook.com/peafone" target="_blank"><FacebookIcon/></a>
                            <a  href="https://mobile.twitter.com/boys_ada" target="_blank"><TwitterIcon/></a>
                        </div>
                        <SubscribeForm/>
                    </div>
                </div>
            </div> */}
            <div className={styles.copyright}>Copyright © 2022. All Rights Reserved By IronSky</div>
        </footer>
    )
}

export default Footer;