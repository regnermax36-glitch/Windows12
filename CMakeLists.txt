cmake_minimum_required(VERSION 3.25)

project(Windows12_Rebirth VERSION 1.0.0 LANGUAGES CXX)

set(CMAKE_CXX_STANDARD 23)
set(CMAKE_CXX_STANDARD_REQUIRED True)
set(CMAKE_CXX_EXTENSIONS False)

if(MSVC)
    # Added /wd4100 and /wd4189 to explicitly silence the unused variable warnings
    add_compile_options(/W4 /EHsc /std:c++latest /permissive- /wd4100 /wd4189)
    add_compile_definitions(_UNICODE UNICODE)
endif()

add_subdirectory(src/CorePC)
add_subdirectory(src/ShellHost)
add_subdirectory(src/OOBE)
add_subdirectory(src/LocalAI_NPU)

option(BUILD_TESTING "Build unit tests" ON)

if(BUILD_TESTING)
    enable_testing()
    add_subdirectory(tests)
endif()
