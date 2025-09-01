import { FuseNavigation } from '@vesact/web-ui-template';

export const navigation: FuseNavigation[] = [
    {
        id: 'infoscreens',
        translate: 'menuItem.infoscreens',
        type: "item",
        url: "/infoscreens",
        icon: 'desktop_windows'
    },
    {
        id: 'contentManagement',
        translate: 'menuItem.contentManagement.group',
        type: "collapsable",
        icon: 'library_books',
        children: [
            {
                id: 'contentManagement-news',
                translate: 'menuItem.contentManagement.news',
                type: "item",
                url: "/contentManagement/news",
                icon: 'message'
            },
            {
                id: 'contentManagement-videos',
                translate: 'menuItem.contentManagement.videos',
                type: "item",
                url: "/contentManagement/videos",
                icon: 'video_library'
            }
        ]
    },
    {
        id: 'account',
        translate: 'menuItem.account.group',
        type: "group",
        children: [
            {
                id: 'logout',
                translate: 'menuItem.account.logout',
                type: "item",
                url: "/auth/logout",
                icon: 'logout'
            }
        ]
    }
];